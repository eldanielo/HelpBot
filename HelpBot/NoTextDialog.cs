using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json.Linq;

namespace HelpBot
{
    [Serializable]
    public class NoTextDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
                  await context.PostAsync("Sieht aus wie " + await findDocAsync(message.Attachments[0].ContentUrl));
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

    }


}
