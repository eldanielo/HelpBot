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
    [LuisModel("80921404-cd04-4684-8755-911d6094af7a", "8b1e7a6900144fb1aa75afa6d91cdbf5")]
    [Serializable]
    public class LUISDialog : LuisDialog<object>
    {
      
   


        internal static IFormDialog<Diebstahlsanzeige> MakeRootDialog()
        {
            return FormDialog.FromForm(Diebstahlsanzeige.BuildForm, options: FormOptions.PromptInStart);
        }
     
        private async Task DiebstahlsanzeigeComplete(IDialogContext context, IAwaitable<Diebstahlsanzeige> result)
        {
            await context.PostAsync("Ihre Anzeige wurde aufgenommen und die Ermittlungen haben begonnen");
            context.Wait(MessageReceived);
        }


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
           
                string message = $"Sorry I did not understand: " +result.Query;
               
                await context.PostAsync(message);
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
            string ip = GeoLocator.GetIPAddress();
            dynamic location = GeoLocator.getCity();

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
            string ip = GeoLocator.GetIPAddress();
            dynamic location = GeoLocator.getCity();
            string entity = result.Entities[0].Entity;
            string resp = "Die näheste " + entity + " von "  +location.zipCode+ location.cityName +" ist Josef-Holaubek-Platz 1 1090 Wien";
    
                   
            await context.PostAsync(resp);
            context.Wait(MessageReceived);
        }

       
    }
}