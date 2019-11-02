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

        public bool updateColumnInTable<T>(string tableName, string columnName, T value, int id)
        {
            if (this.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = string.Format("UPDATE {0} SET {1} = {2} WHERE Id = ${3}", tableName, columnName, value, id);
                    command.CommandType = DT.CommandType.Text;

                    return command.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }
    }
}