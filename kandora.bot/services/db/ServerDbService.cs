using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using kandora.bot.utils;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
        public static string leagueRoleIdCol = "leagueRoleId";
        public static string leagueNameCol = "leagueName";
        public static string leagueConfigIdCol = "leagueId";

        //ServerUser
        public static string userIdCol = "userId";
        public static string serverIdCol = "serverId";
        public static string isAdminCol = "isAdmin";


        public static Server GetServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {displayNameCol}, {leagueRoleIdCol}, {leagueNameCol}, {leagueConfigIdCol} FROM {serverTableName} WHERE {idCol} = \'{serverId}\'";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string displayName = reader.IsDBNull(0) ? null : reader.GetString(0);
                    string leagueRoleId = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string leagueName = reader.IsDBNull(2) ? null : reader.GetString(2);
                    int leagueConfigId = reader.GetInt32(3);

                    reader.Close();
                    return new Server(serverId, displayName, leagueRoleId, leagueName, leagueConfigId); ;
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
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {leagueRoleIdCol}, {leagueNameCol} , {leagueConfigIdCol} FROM {serverTableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0).Trim();
                    string displayName = reader.GetString(1);
                    string leagueRoleId = reader.GetString(2);
                    string leagueName = reader.GetString(3);
                    int leagueConfigId = reader.GetInt32(4);
                    serverMap.Add(id, new Server(id, displayName, leagueRoleId, leagueName, leagueConfigId));
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

        public static void AddServer(string discordId, string displayName, string leagueRoleId, string leagueName, int leagueConfigId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                var sql = $"INSERT INTO {serverTableName} ({displayNameCol}, {leagueRoleIdCol}, {leagueNameCol}, {idCol}, {leagueConfigIdCol}) " +
                    $"VALUES (@displayName, @leagueRoleId, @leagueName, @discordId, @leagueConfigId);";

                using var command = new NpgsqlCommand(sql, dbCon.Connection);

                command.Parameters.AddWithValue("@displayName", NpgsqlDbType.Varchar, displayName);
                command.Parameters.AddWithValue("@leagueRoleId", NpgsqlDbType.Varchar, leagueRoleId);
                command.Parameters.AddWithValue("@leagueName", NpgsqlDbType.Varchar, leagueName);
                command.Parameters.AddWithValue("@discordId", NpgsqlDbType.Varchar, discordId);
                command.Parameters.AddWithValue("@leagueConfigId", NpgsqlDbType.Integer, leagueConfigId);
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

        public static void SetDisplayName(string serverId, string name)
        {
            UpdateFieldInTable(serverTableName, displayNameCol, serverId, name);
        }
        public static void SetLeagueInfo(string serverId, string name, string id)
        {
            UpdateFieldInTable(serverTableName, leagueNameCol, serverId, name);
            UpdateFieldInTable(serverTableName, leagueRoleIdCol, serverId, id);
        }
        public static void SetLeagueConfig(string serverId, int id)
        {
            UpdateFieldInTable(serverTableName, leagueConfigIdCol, serverId, id);
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

        public static void AddUserToServer(string userId, string serverId, bool isAdmin = false)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerUserTableName} ({userIdCol}, {serverIdCol}, {isAdminCol}) " +
                    $"VALUES (@userId, @serverId, @isAdmin);";

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
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
                command.CommandText = $"DELETE FROM {ServerUserTableName} WHERE {userIdCol} = \'{userId}\' AND {serverIdCol} = \'{serverId}\'";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }
    }
}
