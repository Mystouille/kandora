using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;

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
        private MySqlConnection connection = null;
        public MySqlConnection Connection
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
                connection = new MySqlConnection(connstring);
                connection.Open();
            }

            return true;
        }

        public void Close()
        {
            connection.Close();
        }    
        
        public bool updateColumnInTable<T>(string tableName, string columnName, T value, int id)
        {
            if (this.IsConnect())
            {
                string query = string.Format("UPDATE {0} SET {1} = {2} WHERE Id = ${3}", tableName, columnName, value, id);
                var cmd = new MySqlCommand(query, this.Connection);
                var result = cmd.ExecuteNonQuery();
                return result > 0;
            }
            return false;
        }
    }
}