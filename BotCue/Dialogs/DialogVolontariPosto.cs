using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotCue.Classes;

namespace BotCue.Dialogs
{
    public class DialogVolontariPosto : IDialog<object>
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

            //selezionare evento
            DBConnection db = new DBConnection();

            await context.PostAsync($"Dialog Di Prova Aperto");

            context.Wait(MessageReceivedAsync);
        }
    }
}