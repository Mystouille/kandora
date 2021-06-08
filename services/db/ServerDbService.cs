using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using kandora.bot.utils;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kandora.bot.services
{
    class ServerDbService: DbService
    {
        public static string serverTableName = "[dbo].[Server]";
        public static string ServerUserTableName = "[dbo].[ServerUser]";

        //both
        public static string idCol = "Id";

        //Server
        public static string displayNameCol = "displayName";
        public static string leagueRoleIdCol = "leagueRoleId";
        public static string leagueNameCol = "leagueName";
        public static string targetChannelIdCol = "targetChannelId";
        public static string leagueConfigIdCol = "leagueId";

        //ServerUser
        public static string userIdCol = "userId";
        public static string serverIdCol = "serverId";
        public static string isAdminCol = "isAdmin";
        public static string isOwnerCol = "isOwner";


        public static Server GetServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {displayNameCol}, {targetChannelIdCol}, {leagueRoleIdCol}, {leagueNameCol}, {leagueConfigIdCol} FROM {serverTableName} WHERE {idCol} = {serverId}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string displayName = reader.IsDBNull(0) ? null : reader.GetString(0);
                    string targetChannelId = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string leagueRoleId = reader.IsDBNull(2) ? null : reader.GetString(2);
                    string leagueName = reader.IsDBNull(3) ? null : reader.GetString(3);
                    int leagueConfigId = reader.GetInt32(4);

                    reader.Close();
                    return new Server(serverId, displayName, leagueRoleId, leagueName, targetChannelId, leagueConfigId); ;
                }
                reader.Close();
                throw (new EntityNotFoundException("Server"));
            }
            throw (new DbConnectionException());
        }

        public static bool isUserOnServer(string userId, string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {userIdCol} FROM {ServerUserTableName} WHERE {userIdCol} = {userId} AND {serverIdCol} = {serverId}";
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {targetChannelIdCol}, {leagueRoleIdCol}, {leagueNameCol} , {leagueConfigIdCol} FROM {serverTableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0).Trim();
                    string displayName = reader.GetString(1);
                    string targetChannelId = reader.GetString(2);
                    string leagueRoleId = reader.GetString(3);
                    string leagueName = reader.GetString(4);
                    int leagueConfigId = reader.GetInt32(5);
                    serverMap.Add(id, new Server(id, displayName, leagueRoleId, leagueName, targetChannelId, leagueConfigId));
                }
                reader.Close();

                command.CommandText = $"SELECT {userIdCol}, {serverIdCol}, {isAdminCol}, {isOwnerCol} FROM {ServerUserTableName}";
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string userId = reader.GetString(0).Trim();
                    string serverId = reader.GetString(1).Trim();
                    bool isAdmin = reader.GetBoolean(2);
                    bool isOwner = reader.GetBoolean(3);
                    var user = users[userId];
                    var server = serverMap[serverId];
                    server.Users.Add(user);
                    if (isAdmin)
                    {
                        server.Admins.Add(user);
                    }
                    if (isOwner)
                    {
                        server.Owners.Add(user);
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {serverTableName} ({displayNameCol}, {leagueRoleIdCol}, {leagueNameCol}, {idCol}, {leagueConfigIdCol}) " +
                    $"VALUES (@displayName, @leagueRoleId, @leagueName, @discordId, @leagueConfigId);";

                command.Parameters.Add(new SqlParameter("@displayName", SqlDbType.VarChar)
                {
                    Value = displayName
                });
                command.Parameters.Add(new SqlParameter("@leagueRoleId", SqlDbType.VarChar)
                {
                    Value = leagueRoleId
                });
                command.Parameters.Add(new SqlParameter("@leagueName", SqlDbType.VarChar)
                {
                    Value = leagueName
                });
                command.Parameters.Add(new SqlParameter("@discordId", SqlDbType.VarChar)
                {
                    Value = discordId
                });
                command.Parameters.Add(new SqlParameter("@leagueConfigId", SqlDbType.Int)
                {
                    Value = leagueConfigId
                });
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void SetTargetChannel(string serverId, string channelId)
        {
            UpdateFieldInTable(serverTableName, targetChannelIdCol, serverId, channelId);
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

        public static void AddUserToServer(string userId, string serverId, bool isAdmin = false, bool isOwner = false)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerUserTableName} ({userIdCol}, {serverIdCol}, {isAdminCol}, {isOwnerCol}) " +
                    $"VALUES (@userId, @serverId, @isAdmin, @isOwner);";

                command.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)
                {
                    Value = userId
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = serverId
                });
                command.Parameters.Add(new SqlParameter("@isAdmin", SqlDbType.Bit)
                {
                    Value = isAdmin || Bypass.isMyst(userId)
                });
                command.Parameters.Add(new SqlParameter("@isOwner", SqlDbType.Bit)
                {
                    Value = isOwner || Bypass.isMyst(userId)
                });
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"DELETE FROM {ServerUserTableName} WHERE {userIdCol} = {userId} AND {serverIdCol} = {serverId}";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }
    }
}
