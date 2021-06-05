using kandora.bot.exceptions;
using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace kandora.bot.services
{
    internal class RankingDb
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

        internal static void UpdateRankings(Game game)
        {
            if (!game.IsSignedOff)
            {
                throw (new GameNotSignedOffException());
            }
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
                new Ranking(game.User1Id, rkList1, rkList2.Last(), rkList3.Last(), rkList4.Last(), 1, game.Id, game.Server.Id),
                new Ranking(game.User2Id, rkList2, rkList1.Last(), rkList3.Last(), rkList4.Last(), 2, game.Id, game.Server.Id),
                new Ranking(game.User3Id, rkList3, rkList2.Last(), rkList1.Last(), rkList4.Last(), 3, game.Id, game.Server.Id),
                new Ranking(game.User4Id, rkList4, rkList2.Last(), rkList3.Last(), rkList1.Last(), 4, game.Id, game.Server.Id)
            };

            foreach (var ranking in newRkList)
            {
                AddRanking(ranking);
            }
        }

        internal static bool AddRanking(Ranking rk)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {gameIdCol}, {serverIdCol}) " +
                    $"VALUES ({rk.UserId}, {rk.OldElo}, {rk.NewElo}, {rk.Position}, {rk.GameId}, {rk.ServerId});";

                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery() > 0;
            }
            throw (new DbConnectionException());
        }

        internal static void InitUserRanking(string userId, string serverId)
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
                    Value = Ranking.INITIAL_ELO
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
                    $"WHERE {userIdCol} = {userId} AND {serverIdCol} = {serverId} " +
                    $"ORDER BY {idCol} DESC";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    double oldElo = reader.IsDBNull(1) ? -1 : reader.GetDouble(1);
                    double newElo = reader.IsDBNull(2) ? -1 : reader.GetDouble(2);
                    int position = reader.IsDBNull(3) ? -1 : reader.GetInt32(3);
                    DateTime timestamp = reader.GetDateTime(4);
                    int gameId = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
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
        internal static List<Ranking> GetServerRanking(string serverId)
        {
            List<Ranking> rankingListList = new List<Ranking>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"WHERE {serverIdCol} = {serverId} " +
                    $"ORDER BY {newEloCol} DESC";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string userId = reader.GetString(1);
                    double oldElo = reader.IsDBNull(2) ? -1 : reader.GetDouble(2);
                    double newElo = reader.IsDBNull(3) ? -1 : reader.GetDouble(3);
                    int position = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                    DateTime timestamp = reader.GetDateTime(5);
                    int gameId = reader.IsDBNull(6) ? -1 : reader.GetInt32(6);
                    rankingListList.Add(new Ranking(id, userId, oldElo, newElo, position, timestamp, gameId, serverId));
                }
                reader.Close();
                return rankingListList;
            }
            throw (new DbConnectionException());
        }
    }
}