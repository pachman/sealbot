using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CognitiveBot.BusinessLogic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace CognitiveBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var replacedText = await SynonymReplacer.BigBang(activity.Text);

                Activity reply = activity.CreateReply(replacedText);
                await connector.Conversations.ReplyToActivityAsync(reply);

                //await Conversation.SendAsync(activity, () =>
                //   FormDialog.FromForm(SynonymsParam.BuildForm));
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        
    }


    [Serializable]
    public class SynonymsParam
    {
        public static IForm<SynonymsParam> BuildForm()
        {
            return new FormBuilder<SynonymsParam>()
                .Message("Welcome to synonyms bot")
                .Field(nameof(Text))
                .OnCompletion(Completion)
                .Build();
        }

        private static async Task Completion(IDialogContext context, SynonymsParam WP)
        {
            await context.PostAsync(await WP.BuildResult());
            context.Reset();
        }

        [Prompt("Please enter text to get synonyms text ")]
        public string Text { get; set; }

        public async Task<string> BuildResult()
        {
            var replacedText = await SynonymReplacer.BigBang(Text).ConfigureAwait(false);

            return replacedText;
        }
    }
}