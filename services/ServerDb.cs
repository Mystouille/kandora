using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kandora
{
    class ServerDb
    {
        public static string ServerTableName = "[dbo].[Server]";
        public static string ServerUserTableName = "[dbo].[ServerUser]";

        //both
        public static string idCol = "Id";

        //Server
        public static string displayNameCol = "displayName";
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
                command.CommandText = $"SELECT {displayNameCol}, {targetChannelIdCol} FROM {ServerTableName} WHERE {idCol} = {serverId}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string displayName = reader.GetString(1).Trim();
                    string targetChannelId = reader.GetString(2).Trim();
                    return new Server(serverId, displayName, targetChannelId);
                }
                throw (new EntityNotFoundException("Server"));
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
                command.CommandText = $"SELECT {idCol}, {displayNameCol}, {targetChannelIdCol} FROM {ServerTableName}";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0).Trim();
                    string displayName = reader.GetString(1).Trim();
                    string targetChannelId = reader.GetString(2).Trim();
                    serverMap.Add(id, new Server(id, displayName, targetChannelId));
                }

                command.CommandText = $"SELECT {userIdCol}, {serverIdCol}, {isAdminCol}, {isOwnerCol} FROM {ServerUserTableName}";
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string userId = reader.GetString(0).Trim();
                    string serverId = reader.GetString(1).Trim();
                    bool isAdmin = reader.GetBoolean(2);
                    bool isOwner = reader.GetBoolean(3);
                    serverMap[serverId].Users.Add(users[userId]);
                    if (isAdmin)
                    {
                        serverMap[serverId].Admins.Add(users[userId]);
                    }
                    if (isOwner)
                    {
                        serverMap[serverId].Owners.Add(users[userId]);
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerTableName} ({displayNameCol}, {idCol}) " +
                    $"VALUES (@displayName, @discordId);";

                command.Parameters.Add(new SqlParameter("@displayName", SqlDbType.VarChar)
                {
                    Value = displayName
                });
                command.Parameters.Add(new SqlParameter("@discordId", SqlDbType.BigInt)
                {
                    Value = discordId
                });
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }

        public static void SetTargetChannel(Server server, string channelId)
        {
            UpdateColumnInTable(ServerTableName, targetChannelIdCol, channelId, server.Id);
        }

        public static void AddUserToServer(User user, Server server, bool isAdmin = false, bool isOwner = false)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {ServerUserTableName} ({idCol}, {userIdCol}, {serverIdCol}, {isAdminCol}, {isOwnerCol}) " +
                    $"VALUES (@userId, @serverId, @isAdmin, @isOwner);";

                command.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)
                {
                    Value = user.Id
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = server.Id
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
                }
            }
            throw (new DbConnectionException());
        }
    }
}
