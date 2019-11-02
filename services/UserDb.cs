using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using DT = System.Data;

namespace Kandora
{
    class UserDb
    {

        public static string TableName = "[dbo].[User]";
        public static string idCol = "Id";
        public static string displayNameCol = "displayName";
        public static string mahjsoulIdCol = "mahjsoulId";
        public static string isAdminCol = "isAdmin";

        public static List<User> GetUsers()
        {
            List<User> userList = new List<User>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {mahjsoulIdCol}, {isAdminCol} FROM {TableName}";
                command.CommandType = DT.CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ulong id = Convert.ToUInt64(reader.GetDecimal(0));
                    string displayName = reader.GetString(1).Trim();
                    int mahjsoulId = reader.GetInt32(2);
                    bool isAdmin = reader.GetBoolean(3);
                    userList.Add(new User(id, displayName, mahjsoulId, isAdmin));
                }
                reader.Close();
            }
            return userList;
        }

        public static bool AddUser(string displayName, ulong discordId, int mahjsoulId)
        {
            var dbCon = DBConnection.Instance();
            bool touchedRecord = false;
            if (dbCon.IsConnect())
            {

                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {TableName} ({displayNameCol}, {idCol}, {mahjsoulIdCol}, {isAdminCol}) " +
                    $"VALUES (\'{displayName}\', {discordId}, {mahjsoulId}, 0);";
                command.CommandType = DT.CommandType.Text;
                touchedRecord = command.ExecuteNonQuery() > 0;
            }
            return touchedRecord;
        }

        internal static bool SetMahjsoulId(ulong id, int value)
        {
            return UpdateColumnInTable<int>("mahjsoulId", value, id);
        }

        internal static bool SetDiplayName(ulong id, string value)
        {
            return UpdateColumnInTable<string>("displayName", value, id);
        }
        internal static bool SetIsAdmin(ulong id, bool value)
        {
            return UpdateColumnInTable<bool>("displayName", value, id);
        }

        private static bool UpdateColumnInTable<T>(string columnName, T value, ulong id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = string.Format("UPDATE {0} SET {1} = {2} WHERE Id = ${3}", TableName, columnName, value, id);
                    command.CommandType = DT.CommandType.Text;

                    return command.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }
    }
}
