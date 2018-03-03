using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotCue.Dialogs
{
    [Serializable]
    public class DialogVolontario : IDialog<object>
    {
        bool asked = false;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var text = activity.Text;

            if (!asked)
            {
                var card = new HeroCard("Lista Azioni");
                card.Buttons = new List<CardAction>()
            {
                new CardAction()
                {
                    Title = "Calendario Mie Disponibilità",
                    Type=ActionTypes.ImBack,
                    Value= "1"
                },
                new CardAction()
                {
                    Title = "Segnalazione Emergenza",
                    Type=ActionTypes.ImBack,
                    Value = "2"
                },
                new CardAction()
                {
                    Title = "Indietro",
                    Type=ActionTypes.ImBack,
                    Value = "3"
                }
            };


                var reply = activity.CreateReply("Apertura pannello Volontario");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(new Attachment()
                {
                    ContentType = HeroCard.ContentType,
                    Content = card
                });
                await context.PostAsync(reply);
                asked = true;
            }
            else
            {
                var ans = activity.Text;
                switch (ans)
                {
                    case "1":
                        var rep = activity.CreateReply("");
                        rep.Attachments.Add(new Attachment()
                        {
                            ContentUrl = "http://danieleitt.altervista.org/altro/hackabot/calendario.jpg",
                            ContentType = "image/jpg",
                            Name = "Calendario mie reperibilità"
                        });
                        await context.PostAsync(rep);
                        context.Done(true);
                        break;
                    case "3":
                        await context.PostAsync("OK, ritorno");
                        context.Done(true);
                        break;
                    default:
                        await context.PostAsync("Al momento non disponibile, selezionare un'altra opzione");
                        break;
                }
                return;
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeToRoot(IDialogContext context, IAwaitable<object> result)
        {
            // Simply end conversation
            context.Done(true);
        }
    }
}