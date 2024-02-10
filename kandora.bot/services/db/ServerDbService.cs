using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using kandora.bot.utils;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace kandora.bot.services
{
    class ServerDbService: DbService
    {
        public static string serverTableName = "Server";
        public static string ServerUserTableName = "ServerUser";

        //both
        public static string idCol = "Id";

        //Server
        public static string displayNameCol = "displayName";
        public static string leaderboardRoleIdCol = "leaderboardRoleId";
        public static string leaderboardNameCol = "leaderboardName";
        public static string leaderboardConfigIdCol = "leaderboardConfigId";
        public static string leagueIdCol = "leagueId";

        //ServerUser
        public static string userIdCol = "userId";
        public static string serverIdCol = "serverId";
        public static string serverUserDisplayNameCol = "displayName";
        public static string isAdminCol = "isAdmin";


        public static Server GetServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {displayNameCol}, {leaderboardRoleIdCol}, {leaderboardNameCol}, {leaderboardConfigIdCol}, {leaderboardConfigIdCol} FROM {serverTableName} WHERE {idCol} = \'{serverId}\'";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string displayName = reader.IsDBNull(0) ? null : reader.GetString(0);
                    string leaderboardRoleId = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string leaderboardName = reader.IsDBNull(2) ? null : reader.GetString(2);
                    int leaderBoardConfigId = reader.IsDBNull(3) ? -1 : reader.GetInt32(3);
                    int leaderboardConfigId = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);

                    reader.Close();
                    return new Server(serverId, displayName, leaderboardRoleId, leaderboardName, leaderboardConfigId == -1 ? null : leaderboardConfigId, leaderBoardConfigId == -1 ? null: leaderBoardConfigId); ;
                }
                reader.Close();
                throw (new EntityNotFoundException("Server", serverId));
            }
            throw (new DbConnectionException());
        }

        public static bool isUserOnServer(string userId, string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {userIdCol} FROM {ServerUserTableName} WHERE {userIdCol} = \'{userId}\' AND {serverIdCol} = \'{serverId}\'";
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

        public static Dictionary<string, Server> GetServers(Dictionary<string, User> users)
        {
            Dictionary<string,Server> serverMap = new Dictionary<string, Server>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {leaderboardRoleIdCol}, {leaderboardNameCol} , {leaderboardConfigIdCol}, {leaderboardConfigIdCol} FROM {serverTableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0).Trim();
                    string displayName = reader.GetString(1);
                    string leaderboardRoleId = reader.IsDBNull(2) ? null : reader.GetString(2);
                    string leaderboardName = reader.IsDBNull(3) ? null : reader.GetString(3);
                    int leaderBoardConfigId = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                    int leaderboardConfigId = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
                    serverMap.Add(id, new Server(id, displayName, leaderboardRoleId, leaderboardName, leaderboardConfigId == -1 ? null : leaderboardConfigId, leaderBoardConfigId == -1 ? null : leaderBoardConfigId));
                }
                reader.Close();

                command.CommandText = $"SELECT {userIdCol}, {serverIdCol}, {isAdminCol} FROM {ServerUserTableName}";
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string userId = reader.GetString(0).Trim();
                    string serverId = reader.GetString(1).Trim();
                    bool isAdmin = reader.GetBoolean(2);
                    var user = users[userId];
                    var server = serverMap[serverId];
                    server.Users.Add(user);
                    if (isAdmin)
                    {
                        server.Admins.Add(user);
                    }
                }
                reader.Close();
                return serverMap;
            }
            throw (new DbConnectionException());
        }

        public static void AddServer(string discordId, string displayName)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                var sql = $"INSERT INTO {serverTableName} ({idCol}, {displayNameCol}) " +
                    $"VALUES (@discordId, @displayName);";

                using var command = new NpgsqlCommand(sql, dbCon.Connection);

                command.Parameters.AddWithValue("@displayName", NpgsqlDbType.Varchar, displayName);
                command.Parameters.AddWithValue("@discordId", NpgsqlDbType.Varchar, discordId);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void StartLeaderboardOnServer(string serverId, int leaderboardConfigId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"UPDATE {serverTableName} SET {leaderboardConfigIdCol} = @configId WHERE {idCol} = @serverId;";

                command.Parameters.AddWithValue("@configId", NpgsqlDbType.Integer, leaderboardConfigId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void DeleteServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                var sql = $"DELETE FROM {serverTableName} WHERE {idCol} = \'{serverId}\';";
                using var command = new NpgsqlCommand(sql, dbCon.Connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void DeleteUsersFromServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                var sql = $"DELETE FROM {ServerUserTableName} WHERE {serverIdCol} = \'{serverId}\';";
                using var command = new NpgsqlCommand(sql, dbCon.Connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }


        public static void SetUserDisplayName(string serverId, string userId, string name)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"UPDATE {ServerUserTableName} SET {serverUserDisplayNameCol} = @name WHERE {serverIdCol} = @serverId AND {userIdCol} = @userId;";

                command.Parameters.AddWithValue("@name", NpgsqlDbType.Varchar, name);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void SetDisplayName(string serverId, string name)
        {
            UpdateFieldInTable(serverTableName, displayNameCol, serverId, name);
        }

        public static void SetLeaderboardInfo(string serverId, string name, string id)
        {
            UpdateFieldInTable(serverTableName, leaderboardNameCol, serverId, name);
            UpdateFieldInTable(serverTableName, leaderboardRoleIdCol, serverId, id);
        }
        public static void SetLeaderboardConfig(string serverId, int id)
        {
            UpdateFieldInTable(serverTableName, leaderboardConfigIdCol, serverId, id);
        }
        public static void SetIsAdmin(string serverId, string userId, bool isAdmin)
        {
            SetServerUserRole(serverId, userId, isAdminCol, isAdmin);
        }

        private static void SetServerUserRole(string serverId, string userId, string roleColumn, bool roleFlag)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"UPDATE {ServerUserTableName} SET {roleColumn} = @roleFlag, WHERE {serverIdCol} = @serverId AND {userIdCol} = @userId";

                command.Parameters.AddWithValue("@roleFlag", NpgsqlDbType.Boolean, roleFlag);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void AddUserToServer(string userId, string serverId, string displayName, bool isAdmin = false)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerUserTableName} ({userIdCol}, {serverIdCol}, {serverUserDisplayNameCol}, {isAdminCol}) " +
                    $"VALUES (@userId, @serverId, @displayName, @isAdmin);";

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@displayName", NpgsqlDbType.Varchar, displayName);
                command.Parameters.AddWithValue("@isAdmin", NpgsqlDbType.Boolean, isAdmin || Bypass.isSuperUser(userId));
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void RemoveUserFromServer(string userId, string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"DELETE FROM {ServerUserTableName} WHERE {userIdCol} = \'{userId}\' AND {serverIdCol} = \'{serverId}\';";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }


        public static bool IsUserInServer(string userId, string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol} FROM {ServerUserTableName} WHERE {userIdCol} = \'{userId}\' AND {serverIdCol} = \'{serverId}\';";
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
