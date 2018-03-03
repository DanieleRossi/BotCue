using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace BotCue.Classes
{
    public class DBConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnection()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "cue";
            uid = "cue";
            password = "cuebot";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                       Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public bool inserisciUtente(Utente user)
        {
            string query = "INSERT INTO `utenti`(`user_id`, `chat_id`, `nome`, `cognome`, `telefono`, `tipo_utente`, `pos_lat`, `pos_long`) VALUES ("+
                user.getUserId() + ", "+ 
                user.getChatId() + ", '"+
                user.getNome() + "', '"+
                user.getCognome() + ", '"+
                user.getTelefono() + "', 1, 0, 0)";
            
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();

                return true;
            }

            return false;
        }

        public List<String[]> getIncrociEvento(int id_evento)
        {
            List<String[]> eventi = new List<String[]>();

            string query = "SELECT * FROM incroci WHERE id_evento ="+id_evento;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    eventi.Add(new String[]{
                        "Incrocio_" + dataReader["id_incrocio"],
                        dataReader["strada"] + ""
                    });
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return eventi;
            }
            else
            {
                return null;
            }
        }

        public Utente getUtente(String user_id)
        { 

            string query = "SELECT * FROM utenti WHERE user_id =" + user_id;
            
            if (this.OpenConnection() == true)
            {
                Utente user = null; ;
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    user = new Utente(int.Parse(user_id), int.Parse(user_id), dataReader["telefono"].ToString(), dataReader["nome"].ToString(), dataReader["cognome"].ToString());
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return user;
            }
            else
            {
                return null;
            }
        }

        public String getNomeStrada(String id_incrocio)
        {
            string query = "SELECT * FROM incroci WHERE id_incrocio =" + id_incrocio;
            String nome = "";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    nome = dataReader["strada"].ToString();
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return nome;
            }
            else
            {
                return null;
            }
        }

        public List<String> getIdUtenti(int id_evento, int id_incrocio)
        {
            List<String> id = new List<String>();

            string query = "SELECT * FROM incroci_utenti WHERE id_evento =" + id_evento + " AND id_incrocio = " + id_incrocio;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    id.Add(dataReader["user_id"].ToString());
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return id;
            }
            else
            {
                return null;
            }
        }

        public List<String[]> getEventi()
        {
            List<String[]> eventi = new List<String[]>();

            string query = "SELECT * FROM evento";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    eventi.Add(new String[]{
                        "Evento_" + dataReader["id_evento"],
                        dataReader["nome"] + ""
                    });
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return eventi;
            }
            else
            {
                return null;
            }
        }

        public bool insersciInteressiUtente(int user_id, List<String> comunita)
        {
            List<int> id = getIdComunita(comunita);

            //open connection
            if (OpenConnection() == true)
            {
                foreach (int id_comunita in id)
                {
                    String query = "INSERT INTO `punti_interesse`(`user_id`, `id_comunita`) VALUES (" + user_id + ", " + id_comunita + ")";
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                }
                CloseConnection();

                return true;
            }

            return false;
        }

        private List<int> getIdComunita(List<String> comunita)
        {
            List<int> id = new List<int>(comunita.Count);
            //Open connection
            if (this.OpenConnection() == true)
            {
                foreach (String com in comunita)
                {
                    string query = "SELECT id FROM comunita WHERE nome LIKE '%" + com + "%'";
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        id.Add(int.Parse(dataReader["id"].ToString()));
                    }

                    //close Data Reader
                    dataReader.Close();
                }

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return id;
            }
            else
            {
                return null;
            }
        }

        public List<String> getComunitaValle()
        {
            List<String> comunita = new List<string>();

            string query = "SELECT * FROM comunita";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    comunita.Add(dataReader["nome"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return comunita;
            }
            else
            {
                return null;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}