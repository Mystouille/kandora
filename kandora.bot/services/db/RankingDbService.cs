using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace kandora.bot.services
{
    internal class RankingDbService: DbService
    {
        private const string tableName = "Ranking";
        private const string idCol = "Id";
        private const string userIdCol = "userId";
        private const string serverIdCol = "serverId";
        private const string oldEloCol = "oldElo";
        private const string newEloCol = "newElo";
        private const string positionCol = "position";
        private const string timeStampCol = "timeStamp";
        private const string gameIdCol = "gameId";

        internal static List<Ranking> UpdateRankings(Game game, LeagueConfig config)
        {
            List<Ranking> rkList1 = GetUserRankingHistory(game.User1Id, game.Server.Id);
            List<Ranking> rkList2 = GetUserRankingHistory(game.User2Id, game.Server.Id);
            List<Ranking> rkList3 = GetUserRankingHistory(game.User3Id, game.Server.Id);
            List<Ranking> rkList4 = GetUserRankingHistory(game.User4Id, game.Server.Id);

            if (rkList1.Count() == 0)
            {
                InitUserRanking(game.User1Id, game.Server.Id, config);
                rkList1 = GetUserRankingHistory(game.User1Id, game.Server.Id);
            }
            if (rkList2.Count() == 0)
            {
                InitUserRanking(game.User2Id, game.Server.Id, config);
                rkList2 = GetUserRankingHistory(game.User2Id, game.Server.Id);
            }
            if (rkList3.Count() == 0)
            {
                InitUserRanking(game.User3Id, game.Server.Id, config);
                rkList3 = GetUserRankingHistory(game.User3Id, game.Server.Id);
            }
            if (rkList4.Count() == 0)
            {
                InitUserRanking(game.User4Id, game.Server.Id, config);
                rkList4 = GetUserRankingHistory(game.User4Id, game.Server.Id);
            }

            

            List<Ranking> newRkList = new()
            {
                new Ranking(game.User1Id, rkList1, rkList2.Last(), rkList3.Last(), rkList4.Last(), game.User1Placement, game.Id, game.Server.Id, config),
                new Ranking(game.User2Id, rkList2, rkList1.Last(), rkList3.Last(), rkList4.Last(), game.User2Placement, game.Id, game.Server.Id, config),
                new Ranking(game.User3Id, rkList3, rkList2.Last(), rkList1.Last(), rkList4.Last(), game.User3Placement, game.Id, game.Server.Id, config),
                new Ranking(game.User4Id, rkList4, rkList2.Last(), rkList3.Last(), rkList1.Last(), game.User4Placement, game.Id, game.Server.Id, config)
            };

            foreach (var ranking in newRkList)
            {
                AddRanking(ranking);
            }
            return newRkList;
        }


        public static bool BackfillRankings(Server server, LeagueConfig config)
        {

            DeleteRankings(server.Id, butNotTheInitial: true);
            var games = ScoreDbService.GetLastNRecordedGame(server);
            foreach(Game game in games)
            {
                UpdateRankings(game, config);
            }
            return true;
        }

        private static bool AddRanking(Ranking rk)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {gameIdCol}, {serverIdCol}) " +
                    $"VALUES (@userId, @oldElo, @newElo, @position, @gameId, @serverId);";

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, rk.UserId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, rk.ServerId);
                command.Parameters.AddWithValue("@position", NpgsqlDbType.Integer, rk.Position);
                command.Parameters.AddWithValue("@gameId", NpgsqlDbType.Varchar, rk.GameId);
                command.Parameters.AddWithValue("@oldElo", NpgsqlDbType.Double, rk.OldRank);
                command.Parameters.AddWithValue("@newElo", NpgsqlDbType.Double, rk.NewRank);
                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery() > 0;
            }
            throw (new DbConnectionException());
        }

        internal static void InitUserRanking(string userId, string serverId, LeagueConfig leagueConfig)
        {
            List<Ranking> userRankings = GetUserRankingHistory(userId, serverId, latest: true);
            if (userRankings.Any())
            {
                throw new Exception("User is already ranked on that server");
            }

            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {serverIdCol}, {newEloCol}) " +
                    $"VALUES (@userId, @serverId, @rank);";

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@rank", NpgsqlDbType.Double, leagueConfig.InitialElo);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        internal static void DeleteRankings(string serverId, bool butNotTheInitial = false)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                if (butNotTheInitial)
                {
                    command.CommandText = $"DELETE FROM  {tableName} WHERE {serverIdCol} = \'{serverId}\' AND {oldEloCol} IS NOT NULL;";
                }
                else
                {
                    command.CommandText = $"DELETE FROM  {tableName} WHERE {serverIdCol} = \'{serverId}\'";
                }
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        internal static List<Ranking> GetUserRankingHistory(string userId, string serverId, bool latest = false)
        {
            List<Ranking> rankingListList = new List<Ranking>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {idCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"WHERE {userIdCol} = @userId AND {serverIdCol} = @serverId " +
                    $"ORDER BY {idCol} ASC";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    float oldElo = reader.IsDBNull(1) ? -1 : (float)reader.GetDouble(1);
                    float newElo = reader.IsDBNull(2) ? -1 : (float)reader.GetDouble(2);
                    int position = reader.IsDBNull(3) ? -1 : reader.GetInt32(3);
                    DateTime timestamp = reader.GetDateTime(4);
                    string gameId = reader.IsDBNull(5) ? null : reader.GetString(5);
                    rankingListList.Add(new Ranking(id, userId, oldElo, newElo, position, timestamp, gameId, serverId));
                    if (latest)
                    {
                        break;
                    }
                }
                reader.Close();
                return rankingListList;
            }
            throw (new DbConnectionException());
        }
        internal static List<Ranking> GetServerRankings(string serverId)
        {
            List<Ranking> rankingListList = new List<Ranking>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {idCol}, {userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"WHERE {serverIdCol} = \'{serverId}\' " +
                    $"ORDER BY {idCol} DESC";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string userId = reader.GetString(1);
                    float oldElo = reader.IsDBNull(2) ? -1 : (float)reader.GetDouble(2);
                    float newElo = reader.IsDBNull(3) ? -1 : (float)reader.GetDouble(3);
                    int position = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                    DateTime timestamp = reader.GetDateTime(5);
                    string gameId = reader.IsDBNull(6) ? null : reader.GetString(6);
                    rankingListList.Add(new Ranking(id, userId, oldElo, newElo, position, timestamp, gameId, serverId));
                }
                reader.Close();
                return rankingListList;
            }
            throw (new DbConnectionException());
        }
    }
}