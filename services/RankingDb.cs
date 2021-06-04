using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Kandora
{
    internal class RankingDb
    {
        private const string tableName = "[dbo].[Ranking]";
        private const string idCol = "Id";
        private const string userIdCol = "userId";
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
            List<Ranking> rkList1 = GetRankingsFor(columnName: userIdCol, value: $"{game.User1Id}");
            List<Ranking> rkList2 = GetRankingsFor(columnName: userIdCol, value: $"{game.User2Id}");
            List<Ranking> rkList3 = GetRankingsFor(columnName: userIdCol, value: $"{game.User3Id}");
            List<Ranking> rkList4 = GetRankingsFor(columnName: userIdCol, value: $"{game.User4Id}");

            if(!rkList1.Any() || !rkList2.Any() || !rkList3.Any() || !rkList4.Any())
            {
                throw (new UserRankingMissingException());
            }

            List<Ranking> newRkList = new List<Ranking>
            {
                new Ranking(game.User1Id, rkList1, rkList2.Last(), rkList3.Last(), rkList4.Last(), 1, game.Id),
                new Ranking(game.User2Id, rkList2, rkList1.Last(), rkList3.Last(), rkList4.Last(), 2, game.Id),
                new Ranking(game.User3Id, rkList3, rkList2.Last(), rkList1.Last(), rkList4.Last(), 3, game.Id),
                new Ranking(game.User4Id, rkList4, rkList2.Last(), rkList3.Last(), rkList1.Last(), 4, game.Id)
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
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {gameIdCol}) " +
                    $"VALUES ({rk.UserId}, {rk.OldElo}, {rk.NewElo}, {rk.Position}, {rk.GameId});";

                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery() > 0;
            }
            throw (new DbConnectionException());
        }

        internal static void InitUserRanking(string targetUserId)
        {
            List<Ranking> userRankings = GetRankingsFor(columnName: userIdCol, value: $"{targetUserId}").Where(x => x.UserId == targetUserId).ToList();
            if (userRankings.Any())
            {
                throw (new UserAlreadyRankedException());
            }

            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({userIdCol}, {newEloCol}) " +
                    $"VALUES ({targetUserId}, {Ranking.INITIAL_ELO});";

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }

        public static List<Ranking> GetUserRankings(string userId)
        {
            return GetRankingsFor(userIdCol, userId);
        }
        private static List<Ranking> GetRankingsFor(string columnName, string value)
        {
            return GetRankingsFor(columnName, new string[] { value });
        }

        private static List<Ranking> GetRankingsFor(string columnName, string[] values)
        {
            List<Ranking> rankingListList = new List<Ranking>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {userIdCol}, {oldEloCol}, {newEloCol}, {positionCol}, {timeStampCol} , {gameIdCol} FROM {tableName} " +
                    $"{buildWhereClause(columnName, values)} " +
                    $"ORDER BY {idCol} ASC";
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
                    rankingListList.Add(new Ranking(id, userId, oldElo, newElo, position, timestamp, gameId));
                }
                reader.Close();
                return rankingListList;
            }
            throw (new DbConnectionException());
        }

        private static string buildWhereClause(string columnName, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WHERE ");
            for(int i = 0; i<values.Length; i++)
            {
                sb.Append($"{columnName} = {values[i]}");
                if (i < values.Length - 1)
                {
                    sb.Append(" OR ");
                }
            }
            return sb.ToString();
        }
    }
}