using System;
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

                await context.PostAsync("Finito");
            });

            form.Configuration.Confirmation = "Tutto corretto?";

            return form.Build();
        }
    };




}