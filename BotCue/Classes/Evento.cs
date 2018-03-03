using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotCue.Classes
{
    public class Evento
    {

        private String nome;

        private DateTime data;

        private TipoComunitaValle valle;

        private String luogo;

        private String descrizione;

        public Evento(string nome, DateTime data, TipoComunitaValle valle, string luogo, String descrizione)
        {
            this.nome = nome;
            this.data = data;
            this.valle = valle;
            this.luogo = luogo;
            this.descrizione = descrizione;
        }


    }
}