using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotCue.Dialogs
{
    [Serializable]
    public class DialogReclutamentoExtra : IDialog<object>
    {
        private bool asked = false;
        private bool first = true;
        private bool second = false;
        private string s;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var text = activity.Text;

            if (asked && (text == "si" || text == "no"))
            {
                await context.PostAsync("Ricevuto");
                context.Done(true);
                return;
            }
            else { asked = false; }


            if (!first)
            {
                //if (!asked)
                //{
                    var card = new HeroCard("Emergenza: " + text);
                    card.Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "Si",
                                Type=ActionTypes.ImBack,
                                Value="si"
                            },
                            new CardAction()
                            {
                                Title = "No",
                                Type=ActionTypes.ImBack,
                                Value="no"
                            }
                        };

                    var reply = activity.CreateReply("Inviare una richiesta di disponibilità extra a tutti i volontari?");
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(new Attachment()
                    {
                        ContentType = HeroCard.ContentType,
                        Content = card
                    });
                    await context.PostAsync(reply);
                    asked = true;
                }
                
            if (first)
            {
                first = false;
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}