using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;

namespace Chat
{
    public class Chat : Hub
    {      
        public async Task Send(string nick, string message)
        {
            DBConnect dbConnect = new DBConnect();
            dbConnect.InsertMessage(nick, message);
            
            await Clients.All.SendAsync("Send", nick, message);
        }
    }

    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnect()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            server = "31.31.196.94";
            database = "u0480826_ellinachat";
            uid = "u0480826_ellina";
            password = "qwer1234";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + 
                               database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            
            connection = new MySqlConnection(connectionString);
        }
        
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
                return false;
            }
            
            return false;
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
                return false;
            }
            
            return false;
        }

        public void InsertMessage(String name, String message)
        {
            string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string query = $"INSERT INTO chat (name, message, dateMessage) VALUES ('{name}', '{message}', '{dateNow}')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                this.CloseConnection();
            }
        }

        public List<string>[] GetMessages()
        {
            string query = "SELECT * FROM chat";
            
            List< string >[] list = new List< string >[3];
            list[0] = new List< string >();
            list[1] = new List< string >();
            list[2] = new List< string >();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    list[0].Add(dataReader["name"] + "");
                    list[1].Add(dataReader["message"] + "");
                    list[2].Add(dataReader["dateMessage"] + "");
                }
                
                dataReader.Close();
                
                cmd.ExecuteNonQuery();
                
                Console.WriteLine();

                this.CloseConnection();
            }

            return list;
        }
    }
}