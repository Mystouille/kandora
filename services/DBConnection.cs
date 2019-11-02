using System;
using System.Configuration;
using System.Data.SqlClient;
using DT = System.Data;

namespace Kandora
{
    public class DBConnection
    {
        private DBConnection()
        {
        }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private SqlConnection connection = null;
        public SqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                string connstring = ConfigurationManager.ConnectionStrings["kandoradb"].ConnectionString;
                connection = new SqlConnection(connstring);
                connection.Open();
            }
            if (Connection.State == DT.ConnectionState.Closed)
            {
                connection.Open();
            }
            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}