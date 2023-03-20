using kandora.bot.exceptions;
using kandora.bot.mahjong.handcalc;
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
        private const string finalScoreCol = "finalScore";
        private const string timeStampCol = "timeStamp";
        private const string gameIdCol = "gameId";

        internal static List<Ranking> UpdateRankings(Game game, LeagueConfig config)
        {
           var dataList = getFullUserGameInfos(game, config);

            // Creating new rankings
            var newRkList = dataList.Select(userData => new Ranking(userData.UserId, dataList, game.Id, game.ServerId, config)).ToArray();

            // this part is for debugging purpose :D  ====>

            var score1 = newRkList[0].Score;
            var score2 = newRkList[1].Score;
            var score3 = newRkList[2].Score;
            var score4 = newRkList[3].Score;
            var sumScore = score1 + score2 + score3 + score4;

            var rank1 = newRkList[0].NewRank;
            var rank2 = newRkList[1].NewRank;
            var rank3 = newRkList[2].NewRank;
            var rank4 = newRkList[3].NewRank;
            var sumNewRank = rank1 + rank2 + rank3 + rank4;

            var oldRank1 = newRkList[0].OldRank;
            var oldRank2 = newRkList[1].OldRank;
            var oldRank3 = newRkList[2].OldRank;
            var oldRank4 = newRkList[3].OldRank;
            var sumOldRank = oldRank1 + oldRank2 + oldRank3 + oldRank4;

            var deltaRanks = sumNewRank - sumOldRank;

            // <==== Now we get back to the important stuff

            foreach (var ranking in newRkList)
            {
                AddNewRanking(ranking);
            }
            return newRkList.ToList();
        }

        private static List<UserGameData> getFullUserGameInfos(Game game, LeagueConfig config)
        {
            var dataList = new List<UserGameData>();

            List<Ranking> rkList1 = GetUserRankingHistory(game.User1Id, game.ServerId);
            List<Ranking> rkList2 = GetUserRankingHistory(game.User2Id, game.ServerId);
            List<Ranking> rkList3 = GetUserRankingHistory(game.User3Id, game.ServerId);
            List<Ranking> rkList4 = GetUserRankingHistory(game.User4Id, game.ServerId);

            if (rkList1.Count() == 0)
            {
                InitUserRanking(game.User1Id, game.ServerId, config);
                rkList1 = GetUserRankingHistory(game.User1Id, game.ServerId);
            }
            if (rkList2.Count() == 0)
            {
                InitUserRanking(game.User2Id, game.ServerId, config);
                rkList2 = GetUserRankingHistory(game.User2Id, game.ServerId);
            }
            if (rkList3.Count() == 0)
            {
                InitUserRanking(game.User3Id, game.ServerId, config);
                rkList3 = GetUserRankingHistory(game.User3Id, game.ServerId);
            }
            if (!game.IsSanma && rkList4.Count() == 0)
            {
                InitUserRanking(game.User4Id, game.ServerId, config);
                rkList4 = GetUserRankingHistory(game.User4Id, game.ServerId);
            }

            // Aggregating data needed to compute next ranking

            dataList.Add(new UserGameData(game.User1Id, game.User1Score, game.User1Chombo, rkList1));
            dataList.Add(new UserGameData(game.User2Id, game.User2Score, game.User2Chombo, rkList2));
            dataList.Add(new UserGameData(game.User3Id, game.User3Score, game.User3Chombo, rkList3));
            if (!game.IsSanma)
            {
                dataList.Add(new UserGameData(game.User4Id, game.User4Score, game.User4Chombo, rkList4));
            }

            dataList = dataList.OrderBy(x => x.UserScore).ToList();
            dataList.Reverse();

            Ranking.addPlacementToDataList(dataList, game);

            return dataList;
        }


        public static bool BackfillRankings(string serverId, LeagueConfig config)
        {
            DeleteRankings(serverId, butNotTheInitial: false);
            var games = ScoreDbService.GetLastNRecordedGame(serverId, config);
            // from first to last
            games.Reverse();
            foreach(Game game in games)
            {
                UpdateRankings(game, config);
            }
            return true;
        }

        private static bool AddNewRanking(Ranking rk)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {finalScoreCol}, {gameIdCol}, {serverIdCol}) " +
                    $"VALUES (@userId, @oldElo, @newElo, @position, @finalScore, @gameId, @serverId);";

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, rk.UserId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, rk.ServerId);
                command.Parameters.AddWithValue("@position", NpgsqlDbType.Varchar, rk.Position);
                command.Parameters.AddWithValue("@finalScore", NpgsqlDbType.Integer, rk.ScoreWithBonus);
                command.Parameters.AddWithValue("@gameId", NpgsqlDbType.Integer, rk.GameId);
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
                command.Parameters.AddWithValue("@rank", NpgsqlDbType.Double, leagueConfig.EloSystem == "Full" ? leagueConfig.InitialElo : 0);
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
            List<Ranking> rankingList = new List<Ranking>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {idCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol}, {finalScoreCol} FROM {tableName} " +
                    $"WHERE {userIdCol} = @userId AND {serverIdCol} = @serverId " +
                    $"ORDER BY {idCol} ASC";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    float? oldElo = reader.IsDBNull(1) ? null : (float)reader.GetDouble(1);
                    float newElo = (float)reader.GetDouble(2);
                    string position = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    DateTime timestamp = reader.GetDateTime(4);
                    int gameId = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
                    double finalScore = ((double)reader.GetInt32(6)) / 1000;
                    rankingList.Add(new Ranking(id, userId, oldElo, newElo, position, finalScore, timestamp, gameId, serverId));
                    if (latest)
                    {
                        break;
                    }
                }
                reader.Close();
                return rankingList;
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
                command.CommandText = $"SELECT {idCol}, {userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol}, {finalScoreCol} FROM {tableName} " +
                    $"WHERE {serverIdCol} = \'{serverId}\' " +
                    $"ORDER BY {idCol} DESC";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string userId = reader.GetString(1);
                    float? oldElo = reader.IsDBNull(2) ? null : (float)reader.GetDouble(2);
                    float newElo = reader.IsDBNull(3) ? -1 : (float)reader.GetDouble(3);
                    string position = reader.IsDBNull(4) ? "?" : reader.GetString(4);
                    DateTime timestamp = reader.GetDateTime(5);
                    int gameId = reader.IsDBNull(6) ? -1 : reader.GetInt32(6);
                    double finalScore = ((double)reader.GetInt32(7))/1000;
                    rankingListList.Add(new Ranking(id, userId, oldElo, newElo, position, finalScore, timestamp, gameId, serverId));
                }
                reader.Close();
                return rankingListList;
            }
            throw (new DbConnectionException());
        }
    }
}