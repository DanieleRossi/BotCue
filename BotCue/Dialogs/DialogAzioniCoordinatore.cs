using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotCue.Dialogs
{
    [Serializable]
    public class DialogAzioniCoordinatore : IDialog<object>
    {
        private bool asked = false;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;      
            

            if (!asked)
            {
                var card = new HeroCard("Lista Azioni");
                card.Buttons = new List<CardAction>()
            {
                new CardAction()
                {
                    Title = "Calendario Disponibilità Volontari",
                    Type=ActionTypes.ImBack,
                    Value= "1"
                },
                new CardAction()
                {
                    Title = "Verifica Arrivo Personale Sul Posto",
                    Type=ActionTypes.ImBack,
                    Value = "/volontari"
                },
                new CardAction()
                {
                    Title = "Reclutamento Straordinario",
                    Type=ActionTypes.ImBack,
                    Value = "3"
                },
                new CardAction()
                {
                    Title = "Indietro",
                    Type=ActionTypes.ImBack,
                    Value = "4"
                }
            };
                

                var reply = activity.CreateReply("Apertura pannello coordinatore");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(new Attachment()
                {
                    ContentType = HeroCard.ContentType,
                    Content = card
                });
                await context.PostAsync(reply);
                asked = true;
            } else
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
                            Name = "Calendario reperibilità volontari"
                        });
                        await context.PostAsync(rep);
                        context.Done(true);
                        break;
                    case "/volontari":
                        await context.Forward(new DialogVolontariPosto(), ResumeToRoot, activity, CancellationToken.None);
                        break;
                    case "3":
                        await context.PostAsync("Descrivere la tipologia dell'emergenza");
                        await context.Forward(new DialogReclutamentoExtra(), ResumeToRoot, activity, CancellationToken.None);
                        break;
                    case "4":
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