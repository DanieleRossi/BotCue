using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Web.Http.Description;
using System;

namespace BotCue.Controllers
{
    public class NotifyController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Notify()
        {
            string serviceUrl = "https://telegram.botframework.com";
            string userId = "471189957";
            string conversationId = "471189957";
            string botId = "DanieleTest_bot";
            string botName = "BotCueTest";
            string channelId = "telegram";

            var client = new ConnectorClient(new Uri(serviceUrl));

            // Fix for 401 error
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);


            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = new ChannelAccount(botId, botName); ;
            message.Recipient = new ChannelAccount(userId, ""); ;
            message.Conversation = new ConversationAccount(id: conversationId);


            message.Text = "Evento";
            message.Locale = "it-IT";

            try
            {
                await client.Conversations.SendToConversationAsync((Activity)message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
