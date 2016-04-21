using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;

namespace HelpBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Attachments.Count > 0) {
                return await Conversation.SendAsync(message.CreateReplyMessage(await findDoc(message.Attachments[0].ContentUrl)), () => new LUIS());


            }
            return await Conversation.SendAsync(message, () => new LUIS());

        }
        static List<String> docs = new List<string> { "geburtsurkunde", "heiratsurkunde", "sterbeurkunde", "führerschein", "reisepass", "personalausweis", "identitätsausweis", "staatsbürgerschaftsnachweis" };
        static VisionServiceClient visionClient = new VisionServiceClient("9cd97d789a4b4f019dd1770d0a516a1b");

        private async Task<string> findDoc(string url) {

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

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}