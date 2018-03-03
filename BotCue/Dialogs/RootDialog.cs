using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

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

            if (activity.Text.Equals("/start"))
            {
                await context.Forward(new DialogSegnalazione(), ResumeToRoot, activity, CancellationToken.None);
                return;
            }

            if (activity.Text.Equals("/volontari"))
            {
                await context.Forward(new DialogVolontariPosto(), ResumeToRoot, activity, CancellationToken.None);
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