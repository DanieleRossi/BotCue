using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotCue.Classes;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;

namespace BotCue.Dialogs
{


    public enum locDiValle
    {
        Valsugana, Vallagarina, Paganella, Primiero, Rotaliana
    };
    public enum EventOptions { Meteo, Traffico, Manifestazioni };

    [Serializable]
    public class UserType
    {


        [Prompt("In quale valle abiti? {||}")]
        public locDiValle? Valle;
        [Prompt("Quali notizie vuoi ricevere? {||}")]
        public EventOptions? Evento;

        public static IForm<UserType> BuildForm()
        {
            FormBuilder<UserType> form = new FormBuilder<UserType>();

            form.Message("Benvenuto nel Bot!");

            form.OnCompletion(async (context, userType) =>
            {
                context.PrivateConversationData.SetValue<bool>("ProfileComplete", true);
                context.PrivateConversationData.SetValue<locDiValle?>("Valle", userType.Valle);
                context.PrivateConversationData.SetValue<EventOptions?>("EventOptions", userType.Evento);

                await context.PostAsync("Da ora in poi riceverai notifiche come questa: ");

                string serviceUrl = "https://telegram.botframework.com";
                string userId = context.Activity.From.Id;
                string conversationId = context.Activity.From.Id;
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
                String text = "", urlImg = "";

                //open data per allerte meteo, traffico o manifestazioni, creiamo noi da codice la notifica per la simulazione da mostrare al cliente
                switch (userType.Evento)
                {
                    case EventOptions.Meteo:
                        text = "Allerta meteo di grado rosso\n";
                        urlImg = "http://danieleitt.altervista.org/altro/hackabot/meteo.jpg";
                        break;
                    case EventOptions.Traffico:
                        text = "Strada chiusa \n\nS.P.N.101 Val di Cembra dal Km 0 + 046 al Km 0 + 414 località Faver nel Comune di Altavalle. \n\nDal 1 / 3 al 1 / 5 2018";
                        urlImg = "http://danieleitt.altervista.org/altro/hackabot/strada.jpg";
                        break;
                    case EventOptions.Manifestazioni:
                        text = "Adunata degli alpini il 13/05/2018";
                        urlImg = "http://danieleitt.altervista.org/altro/hackabot/evento.jpg";
                        break;
                }

                message.Text = "";
                message.Locale = "it-IT";
                message.Attachments.Add(new Attachment()
                {
                    ContentUrl = urlImg,
                    ContentType = "image/jpg",
                    Name = text
                });

                try
                {
                    if(urlImg != "")
                        await client.Conversations.SendToConversationAsync((Activity)message).ConfigureAwait(false);
                }
                catch (Microsoft.Rest.HttpOperationException httpEx)
                {
                }

            });

            form.Configuration.Confirmation = "Tutto corretto?";

            return form.Build();
        }
    };

}