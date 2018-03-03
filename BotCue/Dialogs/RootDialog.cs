using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using BotCue.Classes;

namespace BotCue.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static IDialog<BotCue.Dialogs.UserType> MakeFormDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BotCue.Dialogs.UserType.BuildForm));
        }


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            StateClient sc = activity.GetStateClient();
            BotData userData = sc.BotState.GetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
            var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");

            if (activity.Text.Equals("/conf") ||
                !boolProfileComplete)
            {
                await context.Forward(FormDialog.FromForm(BotCue.Dialogs.UserType.BuildForm), ResumeToRoot, activity, CancellationToken.None);
                return;
            }

            /*if (activity.Text.Equals("/start"))
            {
                await context.Forward(new DialogSegnalazione(), ResumeToRoot, activity, CancellationToken.None);
                return;
            }*/

            if (activity.Text.Equals("/volontari"))
            {
                await context.Forward(new DialogVolontariPosto(), ResumeToRoot, activity, CancellationToken.None);
                return;
            }

            if (activity.Text.Contains("RispostaVolontariPosto"))
            {
                String id = activity.Text.Split('_')[0];
                String arrivato = activity.Text.Split('_')[2];
                String strada = activity.Text.Split('_')[3];
                String risposta = "";

                Utente user = new DBConnection().getUtente(activity.From.Id);

                risposta = user.getNome() + " " + user.getCognome() + ": ";

                if(arrivato == "Si")
                {
                    risposta += "sono arrivato a ";
                }
                else
                {
                    risposta += "non sono ancora arrivato a ";
                }

                risposta += strada;

                string serviceUrl = "https://telegram.botframework.com";
                string userId = id;
                string conversationId = id;
                string botId = "DanieleTest_bot";
                string botName = "BotCueTest";
                string channelId = "telegram";

                var client = new ConnectorClient(new Uri(serviceUrl));

                // Fix for 401 error
                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);


                IMessageActivity message = Activity.CreateMessageActivity();
                message.From = new ChannelAccount(botId, botName); ;
                message.Recipient = new ChannelAccount(userId); ;
                message.Conversation = new ConversationAccount(id: conversationId);

                message.Text = risposta;
                message.Locale = "it-IT";

                try
                {
                    await client.Conversations.SendToConversationAsync((Activity)message).ConfigureAwait(false);
                }
                catch (Microsoft.Rest.HttpOperationException httpEx)
                {
                }

                return;
            }

            await context.PostAsync("Mi spiace non ho capito");

            //StateClient sc = activity.GetStateClient();
            //BotData userData = sc.BotState.GetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
            //var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");

            //if (!boolProfileComplete)
            //{
            //    await Conversation.SendAsync(activity, MakeFormDialog);
            //}
            //else
            //{
            //    await Conversation.SendAsync(activity, () => new Hackaton.Dialogs.DialogSegnalazione());
            //}

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeToRoot(IDialogContext context, IAwaitable<object> result)
        {
            // Simply end conversation
            context.Done(true);
        }
    }
}