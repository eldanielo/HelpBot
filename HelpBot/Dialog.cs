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
                PostAndWait(context, "Sieht aus wie " + await findDocAsync(message.Attachments[0].ContentUrl));
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
                            geo = model.entities.FirstOrDefault(e => e.type == "builtin.geography.city").entity;
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