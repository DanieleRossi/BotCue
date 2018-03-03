using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotCue.Dialogs
{

    [Serializable]
    public class DialogSegnalazione : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string text = activity.Text.ToLowerInvariant();

            if (text == "fine")
            {
                await context.PostAsync("sono uscito da prova");
                context.Done(true);
                return;
            }

            // return our reply to the user
            await context.PostAsync($"Dialog Di Prova Aperto");

            context.Wait(MessageReceivedAsync);
        }
    }
}