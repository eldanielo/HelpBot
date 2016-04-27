using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace HelpBot
{
    [Serializable]
    public class Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
             context.Wait(MessageReceivedAsync);
        }

      
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
       
            string resp = "";
            LUISModel model = await GetEntityFromLUIS(message.Text);
            if (message.Attachments.Count > 0) {
                string testurl = "https://upload.wikimedia.org/wikipedia/commons/3/3f/A_Licence_2013_Front.jpg";
                string doc  = await findDocAsync(testurl);
                PostAndWait(context, "Sieht aus wie " + doc);
                return;
            }
            if (model.intents.Count() > 0)
            {
                switch (model.intents[0].intent)
                {
                    case "hi":
                       
                        await context.PostAsync("Hallo! Versuch mal: Where is the nearest police station?");
                        await context.PostAsync("Oder: Can I park here?");
                        await context.PostAsync("Oder: I was robbed!");
                        context.Wait(MessageReceivedAsync);
                        break;
                    case "howareyou":
                        PostAndWait(context, "gut, und dir?");
                        break;
                    case "find":
                        string entity = "";
                        if (model.entities.Count() > 0)
                        {
                            entity = model.entities.FirstOrDefault(e => e.type == "Amt").entity;
                            resp = "Die näheste " + entity + " von Ihrer Position (" + GeoLocator.getCity().zipCode + " " + GeoLocator.getCity().cityName + ") ist Josef-Holaubek-Platz 1 1090 Wien (DUMMY DATEN)";

                        }
                        else
                        {
                            resp = "Ich konnt das leider nicht finden";
                       
                        }
                        PostAndWait(context, resp);
                        break;
                    case "ReportTheft":
                        IFormDialog<Diebstahlsanzeige> tmp = MakeRootDialog();
                        context.Call(tmp, DiebstahlsanzeigeComplete);
                        break;
                    case "park":

                        string isKurzpark = "keine";
                        string geo = "";
                        if (model.entities.Count() > 0)
                        {

                            var geoEntity = model.entities.FirstOrDefault(e => e.type == "builtin.geography.city");
                            if(geoEntity == null){
                                geoEntity = model.entities.FirstOrDefault(e => e.type == "location");
                            }
                            if (geoEntity == null)
                            {
                                PostAndWait(context, "Ort nicht gefunden");
                                return;
                            }
                            else {
                                geo = geoEntity.entity;
                            }

                        }
                        if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 22)
                        {
                            isKurzpark = "";
                        }
                        if (geo == "")
                        {
                            geo = GeoLocator.getCity().zipCode + " " + GeoLocator.getCity().cityName;
                        }                   
                        resp = "In " + geo + " ist jetzt (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ") " + isKurzpark + "Kurzparkzone";
                        PostAndWait(context, resp);
                        break;
                    case "MarkOnDoc":
                        if (model.entities.Count() > 0)
                        {
                            var target = model.entities.Where(a => a.type == "TargetAttribute").FirstOrDefault();
                            string targetAttribute = null;
                            if (target != null)
                            {
                                targetAttribute = target.entity;
                            }
                            else {

                                PostAndWait(context, "Das habe ich leider nicht verstanden");
                                return;

                            }
                            var foundDoc = model.entities.Where(a => a.type =="DocumentType").First().entity.ToLower();
                            Document d = Document.documents().Where(x => x.names.Contains(foundDoc)).FirstOrDefault();

                            if (d!= null) {
                                int line = -1;
                                d.attributes.TryGetValue(targetAttribute, out line);
                                line = line == 0 ? -1:line;
                                string respUrl =  await MarkLine(line, d.url, targetAttribute);
                                Message msg = new Message();
                                msg.Text = "Hier finden Sie " + targetAttribute + " ihres " + d.names.First();
                                if (respUrl == "nicht gefunden") {
                                    msg.Text = "Konnte " + targetAttribute + " ihres " + d.names.First() + " nicht finden";
                                }
                              
                                List<Attachment> a = new List<Attachment>();
                                a.Add(new Attachment(contentUrl: respUrl, contentType: "image/jpeg"));

                                msg.Attachments = a;
                                PostAndWait(context, msg);

                            }
                            else { 
                            PostAndWait(context, "Dieses Document kenne ich nicht");
                            }

                        }
                        break;
                    default:
                        PostAndWait(context, "Das habe ich leider nicht verstanden");
                        break;
                }
            }
            else
            {
                PostAndWait(context, "Das habe ich leider nicht verstanden");
            }

        }

        private async void PostAndWait(IDialogContext context, string resp)
        {

            await context.PostAsync(resp);
    
            context.Wait(MessageReceivedAsync);
        }
        private async void PostAndWait(IDialogContext context, Message resp)
        {
         
            await context.PostAsync(resp);
   
            context.Wait(MessageReceivedAsync);
        }



        static List<String> docs = new List<string> { "geburtsurkunde", "heiratsurkunde", "sterbeurkunde", "führerschein", "reisepass", "personalausweis", "identitätsausweis", "staatsbürgerschaftsnachweis" };
        static VisionServiceClient visionClient = new VisionServiceClient("9cd97d789a4b4f019dd1770d0a516a1b");
        private async Task<string> findDocAsync(string url)
        {

            OcrResults analysisResult = await visionClient.RecognizeTextAsync(url);
            foreach (var region in analysisResult.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        foreach (var d in docs)
                        {
                            if (d.Equals(word.Text.ToLower()))
                            {
                                return d;
                            }
                        }
                    }
                }

            }
            return "leider nichts gefunden";
        }

        private async Task<String> drawRectanlge(string url, Microsoft.ProjectOxford.Vision.Contract.Rectangle rectangle) {

            //draw rectangles
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(url);
            MemoryStream ms = new MemoryStream(bytes);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);



            using (var graphics = Graphics.FromImage(img))
            {
                var rect = new System.Drawing.Rectangle(

                rectangle.Left,

                 rectangle.Top,

                 rectangle.Width ,

                 rectangle.Height 

                 );
                graphics.DrawRectangle(new Pen(Brushes.Red, 2), rect);
                

            }

            return await upload(ToStream(img, ImageFormat.Jpeg));

        }
        public static Stream ToStream(Image image, ImageFormat formaw)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }
        private async Task<string> upload(Stream photo)
        {
            var container = await ReadOrCreateContainerAsync("images");
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            string imageName = result;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName+".jpg");
            await blockBlob.UploadFromStreamAsync(photo);
            return blockBlob.Uri.ToString();
        }
        public static async Task<CloudBlobContainer> ReadOrCreateContainerAsync(string containerName)
        {
            var container = Client.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            return container;
        }

        private static CloudBlobClient client;
        private static CloudBlobClient Client
        {
            get
            {
                if (client == null)
                {
                    // Retrieve storage account from connection string.
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        "DefaultEndpointsProtocol=https;AccountName=helpgvstorage;AccountKey=D3iOx741aW/ddN7Qh2Rd0pmMX6BIgsqjKo9qUzZavV+SJE4eVVz5oBoe0Nol+eYfbTtz/UxsBxb2SQeBP4KvvQ==");

                    // Create the blob client.
                    client = storageAccount.CreateCloudBlobClient();
                }

                return client;
            }
        }

        private async Task<string> MarkLine(int linenum, string url, string targetAttribute)
        {
            OcrResults analysisResult = await visionClient.RecognizeTextAsync(url);

            if (linenum >= 0 && linenum < analysisResult.Regions[0].Lines.Count())
            {
                return await drawRectanlge(url, analysisResult.Regions[0].Lines[linenum].Rectangle);
            }
         
                foreach (var region in analysisResult.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            
                                if (word.Text.ToLower().Equals(targetAttribute.ToLower()))
                                {
                                    return await drawRectanlge(url, line.Rectangle);
                                }
                            
                        }
                    }

                }
            
          
            return "nicht gefunden";
        
        }
        private async Task DiebstahlsanzeigeComplete(IDialogContext context, IAwaitable<Diebstahlsanzeige> result)
        {
            await context.PostAsync("Ihre Anzeige wurde aufgenommen und die Ermittlungen haben begonnen");
            context.Wait(MessageReceivedAsync);
        }

        internal static IFormDialog<Diebstahlsanzeige> MakeRootDialog()
        {
            return FormDialog.FromForm(Diebstahlsanzeige.BuildForm, options: FormOptions.PromptInStart);
        }

        private static async Task<LUISModel> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LUISModel Data = new LUISModel();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=127cfb94-33c2-48c8-b135-6151fd38aaa7&subscription-key=40a189ba4f8a4824b4e3371ca059a82b&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LUISModel>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}