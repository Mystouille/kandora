﻿using System;
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
                command.CommandType = CommandType.Text;

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
                return userList;
            }
            throw (new DbConnectionException());
        }

        public static void AddUser(string displayName, ulong discordId, int mahjsoulId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {TableName} ({displayNameCol}, {idCol}, {mahjsoulIdCol}, {isAdminCol}) " +
                    $"VALUES (@displayName, @discordId, @mahjsoulId, 0);";

                //sql injection protection
                command.Parameters.Add(new SqlParameter("@displayName", SqlDbType.VarChar)
                {
                    Value = displayName
                });
                command.Parameters.Add(new SqlParameter("@discordId", SqlDbType.BigInt)
                {
                    Value = discordId
                });
                command.Parameters.Add(new SqlParameter("@mahjsoulId", SqlDbType.Int)
                {
                    Value = mahjsoulId
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

        internal static void SetDiplayName(ulong id, string value)
        {
            UpdateColumnInTable("displayName", $"\'{value}\'", id);
        }
        internal static void SetIsAdmin(ulong id, bool value)
        {
            UpdateColumnInTable("isAdmin", value? "1" : "0", id);
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
