using Npgsql;
using System.Configuration;
using DT = System.Data;

namespace kandora.bot.services
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
        private NpgsqlConnection connection = null;
        public NpgsqlConnection Connection
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
                connection = new NpgsqlConnection(connstring);
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