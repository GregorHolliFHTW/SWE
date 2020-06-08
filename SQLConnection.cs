using BIF.SWE1.Interfaces;
using System;
using System.Xml;
using MySql.Data.MySqlClient;

namespace myWebServer
{
    public class SQLConnection
    {
        private static SQLConnection instance;
        private string MySQLconnection = "user=root;" +
        "server=localhost;" +
        "database=software;" +
        "port=3307";


        private SQLConnection() { }


        public static SQLConnection Instance //singleton instance -> only one instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLConnection();
                }
                return instance;
            }
        }

        public MySqlConnection Connection() //new MySqlConnection, return string MySqlConnection
        {
            return new MySqlConnection(MySQLconnection);
        }
    }

}