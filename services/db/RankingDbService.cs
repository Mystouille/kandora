using kandora.bot.exceptions;
using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace kandora.bot.services
{
    internal class RankingDbService
    {
        private const string tableName = "[dbo].[Ranking]";
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

            if (!rkList1.Any() || !rkList2.Any() || !rkList3.Any() || !rkList4.Any())
            {
                throw (new UserRankingMissingException());
            }

            List<Ranking> newRkList = new List<Ranking>
            {
                new Ranking(game.User1Id, rkList1, rkList2.Last(), rkList3.Last(), rkList4.Last(), 1, game.Id, game.Server.Id, config),
                new Ranking(game.User2Id, rkList2, rkList1.Last(), rkList3.Last(), rkList4.Last(), 2, game.Id, game.Server.Id, config),
                new Ranking(game.User3Id, rkList3, rkList2.Last(), rkList1.Last(), rkList4.Last(), 3, game.Id, game.Server.Id, config),
                new Ranking(game.User4Id, rkList4, rkList2.Last(), rkList3.Last(), rkList1.Last(), 4, game.Id, game.Server.Id, config)
            };

            foreach (var ranking in newRkList)
            {
                AddRanking(ranking);
            }
            return newRkList;
        }

        private static bool AddRanking(Ranking rk)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {gameIdCol}, {serverIdCol}) " +
                    $"VALUES (@userId, @oldElo, @newElo, @position, @gameId, @serverId);";

                command.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)
                {
                    Value = rk.UserId
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = rk.ServerId
                });
                command.Parameters.Add(new SqlParameter("@position", SqlDbType.Int)
                {
                    Value = rk.Position
                });
                command.Parameters.Add(new SqlParameter("@gameId", SqlDbType.VarChar)
                {
                    Value = rk.GameId
                });
                command.Parameters.Add(new SqlParameter("@oldElo", SqlDbType.Float)
                {
                    Value = rk.OldRank
                });
                command.Parameters.Add(new SqlParameter("@newElo", SqlDbType.Float)
                {
                    Value = rk.NewRank
                });
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
                throw (new UserAlreadyRankedException());
            }

            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {serverIdCol}, {newEloCol}) " +
                    $"VALUES (@userId, @serverId, @rank);";

                command.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)
                {
                    Value = userId
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = serverId
                });
                command.Parameters.Add(new SqlParameter("@rank", SqlDbType.Float)
                {
                    Value = leagueConfig.InitialElo
                });
                command.CommandType = CommandType.Text;
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"WHERE {userIdCol} = @userId AND {serverIdCol} = @serverId " +
                    $"ORDER BY {idCol} DESC";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)
                {
                    Value = userId
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = serverId
                });
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"WHERE {serverIdCol} = {serverId} " +
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