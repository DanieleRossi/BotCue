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
    [Serializable]
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


            if (text == "/volontari")
            {
                //selezionare evento
                DBConnection db = new DBConnection();
                List<String[]> eventi = db.getEventi();
                var card = new HeroCard("Eventi");
                card.Buttons = new List<CardAction>();
                foreach (String[] evento in eventi)
                {
                    card.Buttons.Add(new CardAction()
                    {
                        Title = evento[1],
                        Type = ActionTypes.ImBack,
                        Value = evento[0]
                    });
                };

                var reply = activity.CreateReply("Scegli un evento");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(new Attachment()
                {
                    ContentType = HeroCard.ContentType,
                    Content = card
                });
                await context.PostAsync(reply);
            }

            String id_evento = "";
            if (text.Contains("evento_"))
            {
                id_evento = text.Split('_')[1];
                DBConnection db = new DBConnection();
               
                List<String[]> eventi = db.getIncrociEvento(int.Parse(id_evento));
                var card = new HeroCard("Incroci");
                card.Buttons = new List<CardAction>();
                foreach (String[] evento in eventi)
                {
                    card.Buttons.Add(new CardAction()
                    {
                        Title = evento[1],
                        Type = ActionTypes.ImBack,
                        Value = evento[0] + "_" + id_evento
                    });
                };

                var reply = activity.CreateReply("Scegli un incrocio");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(new Attachment()
                {
                    ContentType = HeroCard.ContentType,
                    Content = card
                });
                await context.PostAsync(reply);
            }

            String id_incrocio = "";
            if (text.Contains("incrocio_"))
            {
                id_evento = text.Split('_')[2];
                id_incrocio = text.Split('_')[1];
                DBConnection db = new DBConnection();

                String nome_strada = db.getNomeStrada(id_incrocio);

                List<String> id_utenti = db.getIdUtenti(int.Parse(id_evento), int.Parse(id_incrocio));

                foreach(String id in id_utenti)
                {
                    if (activity.From.Id == id)
                        continue;
                    string serviceUrl = "https://telegram.botframework.com";
                    string userId = id;
                    string conversationId = id;
                    string botId = "DanieleTest_bot";
                    string botName = "BotCueTest";
                    string channelId = "telegram";

                    Utente utente = db.getUtente(id);

                    var client = new ConnectorClient(new Uri(serviceUrl));

                    // Fix for 401 error
                    MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);


                    IMessageActivity message = Activity.CreateMessageActivity();
                    message.From = new ChannelAccount(botId, botName); ;
                    message.Recipient = new ChannelAccount(userId); ;
                    message.Conversation = new ConversationAccount(id: conversationId);



                    var card = new HeroCard("Sei arrivato a " + nome_strada + "?");
                    card.Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = "Si",
                            Type=ActionTypes.ImBack,
                            Value= activity.From.Id+"_RispostaVolontariPosto_Si_"+ nome_strada
                        },
                        new CardAction()
                        {
                            Title = "No",
                            Type=ActionTypes.ImBack,
                            Value= activity.From.Id+"_RispostaVolontariPosto_No_"+ nome_strada
                        }
                    };

                    message.Attachments = new List<Attachment>();

                    message.Attachments.Add(new Attachment()
                    {
                        ContentType = HeroCard.ContentType,
                        Content = card
                    });
                    message.Text = "Evento";
                    message.Locale = "it-IT";

                    try
                    {
                        await client.Conversations.SendToConversationAsync((Activity)message).ConfigureAwait(false);
                    }
                    catch (Microsoft.Rest.HttpOperationException httpEx)
                    {
                    }

                }

                await context.PostAsync("Domande inviate");
                context.Done(true);
                return;
            }

        context.Wait(MessageReceivedAsync);
        }
    }
}