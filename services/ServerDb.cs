using kandora.bot.exceptions;
using kandora.bot.models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kandora.bot.services
{
    class ServerDb
    {
        public static string ServerTableName = "[dbo].[Server]";
        public static string ServerUserTableName = "[dbo].[ServerUser]";

        //both
        public static string idCol = "Id";

        //Server
        public static string displayNameCol = "displayName";
        public static string leagueRoleIdCol = "leagueRoleId";
        public static string targetChannelIdCol = "targetChannelId";

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
                command.CommandText = $"SELECT {displayNameCol}, {targetChannelIdCol}, {leagueRoleIdCol} FROM {ServerTableName} WHERE {idCol} = {serverId}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string displayName = reader.IsDBNull(0) ? null : reader.GetString(0).Trim();
                    string targetChannelId = reader.IsDBNull(1) ? null : reader.GetString(1).Trim();
                    string leagueRoleId = reader.IsDBNull(2) ? null : reader.GetString(2).Trim();

                    reader.Close();
                    return new Server(serverId, displayName, leagueRoleId, targetChannelId);
                }
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
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {targetChannelIdCol}, {leagueRoleIdCol} FROM {ServerTableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0).Trim();
                    string displayName = reader.GetString(1).Trim();
                    string targetChannelId = reader.GetString(2).Trim();
                    string leagueRoleId = reader.GetString(3).Trim();
                    serverMap.Add(id, new Server(id, displayName, leagueRoleId, targetChannelId));
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

        public static void AddServer(string discordId, string displayName, string leagueRoleId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerTableName} ({displayNameCol}, {leagueRoleIdCol}, {idCol}) " +
                    $"VALUES (@displayName, @leagueRoleId, @discordId);";

                command.Parameters.Add(new SqlParameter("@displayName", SqlDbType.VarChar)
                {
                    Value = displayName
                });
                command.Parameters.Add(new SqlParameter("@leagueRoleId", SqlDbType.VarChar)
                {
                    Value = leagueRoleId
                });
                command.Parameters.Add(new SqlParameter("@discordId", SqlDbType.BigInt)
                {
                    Value = discordId
                });
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static void SetTargetChannel(string serverId, string channelId)
        {
            UpdateColumnInTable(ServerTableName, targetChannelIdCol, channelId, serverId);
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
                    Value = isAdmin
                });
                command.Parameters.Add(new SqlParameter("@isOwner", SqlDbType.Bit)
                {
                    Value = isOwner
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

        private static void UpdateColumnInTable(string tableName, string columnName, string value, string id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {tableName} SET {columnName} = {value} WHERE Id = {id}";
                    command.CommandType = CommandType.Text;

                    command.ExecuteNonQuery();
                    return;
                }
            }
            throw (new DbConnectionException());
        }
    }
}
