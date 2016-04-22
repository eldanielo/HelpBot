using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json.Linq;

namespace HelpBot
{
    [LuisModel("127cfb94-33c2-48c8-b135-6151fd38aaa7", "7a6c6db56efc41a0ab12e80856fdeb7d")]
    [Serializable]
    public class LUISDialog : LuisDialog<object>
    {
        [Serializable]
        public class PartialMessage
        {
            public string ContentURL { set; get; }

            public string Text { set; get; }

        }

        private PartialMessage message;
   
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<Message> item)
        {
            var msg = await item;
            if (msg.Attachments.Count >  0) {
                this.message = new PartialMessage { ContentURL = msg.Attachments[0].ContentUrl };
            }
            
            await base.MessageReceived(context, item);
        }

        internal static IFormDialog<Diebstahlsanzeige> MakeRootDialog()
        {
            return FormDialog.FromForm(Diebstahlsanzeige.BuildForm, options: FormOptions.PromptInStart);
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
            context.Wait(MessageReceived);
        }


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            if (this.message != null)
            {
                string url = "https://upload.wikimedia.org/wikipedia/commons/3/3f/A_Licence_2013_Front.jpg";
                await context.PostAsync("Sieht aus wie " + await findDocAsync(message.ContentURL));
                this.message = null;
                context.Wait(MessageReceived);
            }
            else { 
                string message = $"Sorry I did not understand: " +result.Query;
               
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }
        }
        [LuisIntent("lawnmowing")]
        public async Task lawnmowing(IDialogContext context, LuisResult result)
        {

            
            string ip = GetIPAddress();

            ip += " \n query: " + result.Query;
            ip += getCity();
            await context.PostAsync("ip"+ ip);
            
            context.Wait(MessageReceived);
        }
        [LuisIntent("howareyou")]
        public async Task howareyou(IDialogContext context, LuisResult result)
        {
            IFormDialog<Diebstahlsanzeige> tmp = MakeRootDialog();
            context.Call(tmp, DiebstahlsanzeigeComplete);
        }
        [LuisIntent("park")]
        public async Task park(IDialogContext context, LuisResult result)
        {
            string ip = GetIPAddress();
            dynamic location = getCity();

            string isKurzpark = "keine";
            if (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 22) {
                isKurzpark = "";
            }
            string resp = "In " + location.zipCode + location.cityName + " ist um " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + isKurzpark+ "Kurzparkzone";
            
            await context.PostAsync(resp);
            context.Wait(MessageReceived);
        }
        [LuisIntent("ReportTheft")]
        public async Task ReportTheft(IDialogContext context, LuisResult result)
        {
            IFormDialog<Diebstahlsanzeige> tmp = MakeRootDialog();
            context.Call(tmp, DiebstahlsanzeigeComplete);
        }

        [LuisIntent("find")]
        public async Task find(IDialogContext context, LuisResult result)
        {
            string ip = GetIPAddress();
            dynamic location = getCity();
            string entity = result.Entities[0].Entity;
            string resp = "Die näheste " + entity + " von "  +location.zipCode+ location.cityName +" ist Josef-Holaubek-Platz 1 1090 Wien";
    
                   
            await context.PostAsync(resp);
            context.Wait(MessageReceived);
        }

        public dynamic getCity()
        {
            //2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf
        
            string s = new WebClient().DownloadString("http://api.ipinfodb.com/v3/ip-city/?key=2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf&format=json");
            dynamic location = JObject.Parse(s);
            return location;
        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

    }
}