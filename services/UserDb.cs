using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kandora
{
    class UserDb
    {

        public static string TableName = "User";

        public static async IAsyncEnumerable<User> getUsers()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                string query = "SELECT Id, displayName, uniqueName, mahjsouId FROM User";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string displayName = reader.GetString(1);
                    string uniqueName = reader.GetString(2);
                    string mahjsoulId = reader.GetString(3);
                    User user = new User(id, displayName, uniqueName, mahjsoulId);
                    yield return user;
                }
                dbCon.Close();
            }
        }

        internal static bool setMahjsoulId(int id, string value)
        {
            var dbCon = DBConnection.Instance();
            return dbCon.updateColumnInTable<string>(TableName, "mahjsoulId", value, id);
        }

        internal static bool setUniqueName(int id, string value)
        {
            var dbCon = DBConnection.Instance();
            return dbCon.updateColumnInTable<string>(TableName, "uniqueName", value, id);
        }

        internal static bool setDiplayName(int id, string value)
        {
            var dbCon = DBConnection.Instance();
            return dbCon.updateColumnInTable<string>(TableName, "displayName", value, id);
        }
    }
}
