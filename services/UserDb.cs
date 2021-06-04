using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kandora
{
    class UserDb
    {

        public static string TableName = "[dbo].[User]";
        public static string idCol = "Id";
        public static string displayNameCol = "displayName";
        public static string mahjsoulIdCol = "mahjsoulId";
        public static string tenhouIdCol = "tenhouId";
        public static string isAdminCol = "isAdmin";

        public static Dictionary<string,User> GetUsers()
        {
            Dictionary<string, User> users = new Dictionary<string, User>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {mahjsoulIdCol}, {tenhouIdCol} FROM {TableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    string displayName = reader.GetString(1).Trim();
                    string mahjsoulId = reader.GetString(2);
                    string tenhouId = reader.GetString(3).Trim();
                    users.Add(id, new User(id, displayName, mahjsoulId, tenhouId));
                }
                reader.Close();
                return users;
            }
            throw (new DbConnectionException());
        }

        public static void AddUser(string displayName, string discordId, string mahjsoulId, string tenhouId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {TableName} ({displayNameCol}, {idCol}, {mahjsoulIdCol}, {tenhouIdCol}) " +
                    $"VALUES (@displayName, @discordId, @mahjsoulId, @tenhouId);";

                //sql injection protection
                command.Parameters.Add(new SqlParameter("@displayName", SqlDbType.VarChar)
                {
                    Value = displayName
                });
                command.Parameters.Add(new SqlParameter("@discordId", SqlDbType.VarChar)
                {
                    Value = discordId
                });
                command.Parameters.Add(new SqlParameter("@mahjsoulId", SqlDbType.VarChar)
                {
                    Value = mahjsoulId
                });
                command.Parameters.Add(new SqlParameter("@tenhouId", SqlDbType.VarChar)
                {
                    Value = tenhouId
                });
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();

                RankingDb.InitUserRanking(discordId);
            }
            throw (new DbConnectionException());
        }

        internal static void SetMahjsoulId(ulong id, int value)
        {
            UpdateColumnInTable("mahjsoulId", $"{value}", id);
        }
        internal static void SetTenhoulId(ulong id, int value)
        {
            UpdateColumnInTable("tenhouId", $"{value}", id);
        }

        internal static void SetDiplayName(ulong id, string value)
        {
            UpdateColumnInTable("displayName", $"\'{value}\'", id);
        }

        private static void UpdateColumnInTable(string columnName, string value, ulong id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {TableName} SET {columnName} = {value} WHERE Id = {id}";
                    command.CommandType = CommandType.Text;

                    command.ExecuteNonQuery();
                }
            }
            throw (new DbConnectionException());
        }
    }
}
