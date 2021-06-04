using DSharpPlus.Entities;
using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Kandora
{
    class ScoreDb
    {
        public static DbDataReader Reader = null;
        private const string GameTableName = "[dbo].[Game]";
        private const string RankingTableName = "[dbo].[Ranking]";

        //Ranking
        private const string RankingTableGameIdCol = "gameId";
        private const string User1Id = "user1Id";
        private const string User1Signed = "user1Signed";
        private const string User2Id = "user2Id";
        private const string User2Signed = "user2Signed";
        private const string User3Id = "user3Id";
        private const string User3Signed = "user3Signed";
        private const string User4Id = "user4Id";
        private const string User4Signed = "user4Signed";
        private const string Timestamp = "timestamp";
        private const string IdCol = "Id";
        private const string ServerIdCol = "serverId";

        public static Game RecordGame(string[] members, string sourceMember, Server server, bool signed = false)
        {
            var dbCon = DBConnection.Instance();

            if (members[0] == members[1] || members[0] == members[2] || members[0] == members[3]
                || members[1] == members[2] || members[1] == members[3]
                || members[2] == members[3])
            {
                throw (new NotEnoughUsersException());
            }

            string bitSigned1 = (signed || sourceMember == members[0]) ? "1" : "0";
            string bitSigned2 = (signed || sourceMember == members[1]) ? "1" : "0";
            string bitSigned3 = (signed || sourceMember == members[2]) ? "1" : "0";
            string bitSigned4 = (signed || sourceMember == members[3]) ? "1" : "0";

            int gameId = GetMaxId() + 1;

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SET IDENTITY_INSERT {GameTableName} ON \n" +
                    $"INSERT INTO {GameTableName} ({IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}) " +
                    $"VALUES ({gameId}, {members[0]}, {members[1]}, {members[2]}, {members[3]}, {bitSigned1}, {bitSigned2}, {bitSigned3}, {bitSigned4});";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();

                var game = GetGame(gameId, server);
                if (signed)
                {
                    RankingDb.UpdateRankings(game);
                }

                return game;
            }
            throw (new DbConnectionException());
        }

        public static void RevertGame(int gameId)
        {
            var dbCon = DBConnection.Instance();

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"DELETE FROM {RankingTableName} WHERE {RankingTableGameIdCol} = {gameId}" +
                    $"DELETE FROM {GameTableName} WHERE {IdCol} = {gameId}";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }

        private static int GetMaxId()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT MAX({IdCol}) FROM {GameTableName}";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.IsDBNull(0) ? 0 : Reader.GetInt32(0);
                    Reader.Close();
                    return id;
                }
            }
            throw (new DbConnectionException());
        }

        public static Game GetLastRecordedGame(Server server)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {ServerIdCol} = {server.Id}" +
                    $" ORDER BY {Timestamp} DESC";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.GetInt32(0);
                    string user1Id = Reader.GetString(1);
                    string user2Id = Reader.GetString(2);
                    string user3Id = Reader.GetString(3);
                    string user4Id = Reader.GetString(4);
                    bool user1Signed = Reader.GetBoolean(5);
                    bool user2Signed = Reader.GetBoolean(6);
                    bool user3Signed = Reader.GetBoolean(7);
                    bool user4Signed = Reader.GetBoolean(8);
                    string serverId = Reader.GetString(9);

                    Reader.Close();
                    return new Game(id, server, user1Id, user2Id, user3Id, user4Id, user1Signed, user2Signed, user3Signed, user4Signed);
                }
            }
            throw (new DbConnectionException());
        }

        public static Game GetGame(int gameId, Server server)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}, {ServerIdCol}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {IdCol} = @gameId";

                //sql injection protection
                command.Parameters.Add(new SqlParameter("@gameId", SqlDbType.Int)
                {
                    Value = gameId
                });

                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.GetInt32(0);
                    string user1Id = Reader.GetString(1);
                    string user2Id = Reader.GetString(2);
                    string user3Id = Reader.GetString(3);
                    string user4Id = Reader.GetString(4);
                    bool user1Signed = Reader.GetBoolean(5);
                    bool user2Signed = Reader.GetBoolean(6);
                    bool user3Signed = Reader.GetBoolean(7);
                    bool user4Signed = Reader.GetBoolean(8);
                    string serverId = Reader.GetString(9);

                    Reader.Close();

                    if(serverId != server.Id)
                    {
                        throw new GetGameException($"Game n°{gameId} is not in this server: {server.DisplayName}");
                    }
                    if(id != gameId)
                    {
                        throw (new GetGameException($"Tried to fetch game n°{gameId} but got n°{id} instead."));
                    }
                    return new Game(id, server, user1Id, user2Id, user3Id, user4Id, user1Signed, user2Signed, user3Signed, user4Signed);
                }
                throw (new GetGameException($"Game n°{gameId} not found"));
            }
            throw (new DbConnectionException());
        }
               
        internal static void SetMahjsoulId(int id, string value)
        {
            UpdateColumnInTable("mahjsoulId", value, id);
        }

        internal static void SignGameByUser(Game game, string userId)
        {
            TrySignGameByUser(userId, game);
            if (game.IsSignedOff)
            {
                RankingDb.UpdateRankings(game);
            }
        }

        public static void TrySignGameByUser(string userId, Game game)
        {

            if (game.User1Id == userId)
            {
                game.User1Signed |= ScoreDb.SignGameByUserPos(game.Id, 1);
            }
            else if (game.User2Id == userId)
            {
                game.User2Signed |= ScoreDb.SignGameByUserPos(game.Id, 2);
            }
            else if (game.User3Id == userId)
            {
                game.User3Signed |= ScoreDb.SignGameByUserPos(game.Id, 3);
            }
            else if (game.User4Id == userId)
            {
                game.User4Signed |= ScoreDb.SignGameByUserPos(game.Id, 4);
            }
            else
            {
                throw (new UserNotFoundInGameException());
            }
        }

        internal static bool SignGameByUserPos(int id, int userPos)
        {
            return UpdateColumnInTable($"user{userPos}Signed", "1", id);
        }

        private static bool UpdateColumnInTable(string columnName, string value, int id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {GameTableName} SET {columnName} = {value} WHERE Id = {id}";
                    command.CommandType = CommandType.Text;

                    return command.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }
    }
}
