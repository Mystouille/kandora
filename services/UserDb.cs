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
        public static string discordIdCol = "discordId";
        public static string mahjsoulIdCol = "mahjsoulId";

        public static List<User> getUsers()
        {
            List<User> userList = new List<User>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {

                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandText = string.Format("SELECT {0}, {1}, {2}, {3} FROM {4}", 
                        idCol,
                        displayNameCol,
                        discordIdCol,
                        mahjsoulIdCol,
                        TableName);
                    command.CommandType = DT.CommandType.Text;

                    try
                    {
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string displayName = reader.GetString(1).Trim();
                            ulong discordId = Convert.ToUInt64(reader.GetString(2).Trim());
                            int mahjsoulId = reader.GetInt32(3);
                            userList.Add(new User(id, displayName, discordId, mahjsoulId));
                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return userList;
        }

        public static bool AddUser(string displayName, ulong discordId, int mahjsoulId)
        {
            var dbCon = DBConnection.Instance();
            bool touchedRecord = false;
            if (dbCon.IsConnect())
            {

                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandText = $"INSERT INTO {TableName} ({displayNameCol}, {discordIdCol}, {mahjsoulIdCol}) " +
                        $"VALUES (\'{displayName}\', \'{discordId.ToString()}\', {mahjsoulId});";
                    command.CommandType = DT.CommandType.Text;
                    touchedRecord = command.ExecuteNonQuery() > 0;
                }
            }
            return touchedRecord;
        }

        internal static bool SetMahjsoulId(int id, int value)
        {
            var dbCon = DBConnection.Instance();
            return dbCon.updateColumnInTable<int>(TableName, "mahjsoulId", value, id);
        }

        internal static bool SetDiplayName(int id, string value)
        {
            var dbCon = DBConnection.Instance();
            return dbCon.updateColumnInTable<string>(TableName, "displayName", value, id);
        }
    }
}
