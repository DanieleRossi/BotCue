using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace BotCue.Classes
{
    public class Utente
    {

        public static int TIPO_COLLABORATORE = 1;
        public static int TIPO_VOLONTARIO = 2;
        public static int TIPO_GUEST = 3;

        private int user_id;
        private int chat_id;

        private String telefono;
        private String nome;
        private String cognome;

        private int tipo_utente;

        private ArrayList luoghiInteresse;

        private double latitudine;
        private double longitudine;

        public Utente(int user_id, int chat_id, string telefono, string nome, string cognome, int tipo_utente = 1)
        {
            this.user_id = user_id;
            this.chat_id = chat_id;
            this.telefono = telefono;
            this.nome = nome;
            this.cognome = cognome;
            tipo_utente = TIPO_GUEST;
            this.tipo_utente = tipo_utente;
        }

        public void setPosizione(double latitudine, double longitudine)
        {
            this.latitudine = latitudine;
            this.longitudine = longitudine;
        }

        public double getLatitudine()
        {
            return latitudine;
        }

        public double getLongitudine()
        {
            return longitudine;
        }

        public void setLuoghiInteresse(params TipoComunitaValle[] luoghi)
        {
            luoghiInteresse = new ArrayList(luoghi.Length);
            foreach(TipoComunitaValle luogo in luoghi)
            {
                luoghiInteresse.Add(luogo);
            }
        }

        public ArrayList getLuoghiInteresse()
        {
            return luoghiInteresse;
        }

        public int getUserId()
        {
            return user_id;
        }

        public int getChatId()
        {
            return chat_id;
        }

        public int getTipoUtente()
        {
            return tipo_utente;
        }

        public String getTelefono()
        {
            return telefono;
        }

        public String getNome()
        {
            return nome;
        }

        public String getCognome()
        {
            return cognome;
        }
    }
}

