using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;

namespace HelpBot
{
    [LuisModel("127cfb94-33c2-48c8-b135-6151fd38aaa7", "7a6c6db56efc41a0ab12e80856fdeb7d")]
    [Serializable]
    public class LUIS : LuisDialog<object>
    {

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));

            message += " \n query: " + result.Query;
                await context.PostAsync(message);
            context.Wait(MessageReceived);
        }


        [LuisIntent("lawnmowing")]
        public async Task FindAlarm(IDialogContext context, LuisResult result)
        {

            
            string ip = GetIPAddress();

            ip += " \n query: " + result.Query;
            ip += getCity();
            await context.PostAsync("ip"+ ip);
            
            context.Wait(MessageReceived);
        }

        public String getCity()
        {
            //2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf

            return new WebClient().DownloadString("http://api.ipinfodb.com/v3/ip-city/?key=2da7d59b916ec038bdb243d2adf389f4958d5f8e9fe8cf6fb838d72cef829bbf");
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