using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kandora.bot.services
{
    class UserDbService: DbService
    {

        public static string tableName = "DiscordUser";
        public static string idCol = "Id";
        public static string mahjsoulNameCol = "mahjsoulName";
        public static string mahjsoulFriendIdCol = "mahjsoulFriendId";
        public static string mahjsoulUserIdCol = "mahjsoulUserId";
        public static string tenhouNameCol = "tenhouName";
        
        public static string isAdminCol = "isAdmin";

        public static Dictionary<string,User> GetUsers()
        {
            Dictionary<string, User> users = new Dictionary<string, User>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {mahjsoulNameCol}, {mahjsoulFriendIdCol}, {mahjsoulUserIdCol}, {tenhouNameCol} FROM {tableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    var user = new User(id);
                    user.MahjsoulName = reader.IsDBNull(1)? null : reader.GetString(1);
                    user.MahjsoulFriendId = reader.IsDBNull(2) ? null : reader.GetString(2);
                    user.MahjsoulUserId = reader.IsDBNull(3) ? null : reader.GetString(3);
                    user.TenhouName = reader.IsDBNull(4) ? null : reader.GetString(4);

                    users.Add(id, user);
                }
                reader.Close();
                return users;
            }
            throw (new DbConnectionException());
        }

        public static bool IsUserInDb(string userId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol} FROM {tableName} WHERE {idCol} = \'{userId}\'";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reader.Close();
                    return true;
                }
                reader.Close();
                return false;
            }
            throw (new DbConnectionException());
        }

        public static void CreateUser(string userDiscordId, string serverId, LeagueConfig leagueConfig)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"INSERT INTO {tableName} ({idCol}) " +
                    $"VALUES (@discordId);";

                command.Parameters.AddWithValue("@discordId", NpgsqlDbType.Varchar, userDiscordId);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                RankingDbService.InitUserRanking(userDiscordId, serverId, leagueConfig);

                return;
            }
            throw (new DbConnectionException());
        }

        public static void SetMahjsoulUserId(string userId, string value)
        {
            UpdateFieldInTable(tableName, mahjsoulUserIdCol, userId, value);
        }
        public static void SetMahjsoulName(string userId, string value)
        {
            UpdateFieldInTable(tableName, mahjsoulNameCol, userId, value);
        }
        public static void SetMahjsoulFriendId(string userId, string value)
        {
            UpdateFieldInTable(tableName, mahjsoulFriendIdCol, userId, value);
        }
        public static void SetTenhouName(string userId, string value)
        {
            UpdateFieldInTable(tableName, tenhouNameCol, userId, value);
        }

        public static bool MahjsoulNameExistAlready(string userId, string serverId, string value)
        {
            return ValueExistAlready(userId, serverId, mahjsoulNameCol, value);
        }

        public static bool MahjsoulFriendIdExistAlready(string userId, string serverId, string value)
        {
            return ValueExistAlready(userId, serverId, mahjsoulFriendIdCol, value);
        }

        public static bool TenhouNameExistAlready(string userId, string serverId, string value)
        {
            return ValueExistAlready(userId, serverId, tenhouNameCol, value);
        }

        private static bool ValueExistAlready(string userId, string serverId, string columnName, string value)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol} FROM {tableName} WHERE {columnName} = \'{value}\' AND {idCol} != \'{userId}\'";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reader.Close();
                    return true;
                }
                reader.Close();
                return false;
            }
            throw (new DbConnectionException());
        }
    }
}
