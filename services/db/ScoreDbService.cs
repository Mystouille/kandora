using kandora.bot.exceptions;
using kandora.bot.http;
using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace kandora.bot.services
{
    class ScoreDbService
    {
        public static DbDataReader Reader = null;
        private const string GameTableName = "[dbo].[Game]";

        //Game
        private const string User1IdCol = "user1Id";
        private const string User2IdCol = "user2Id";
        private const string User3IdCol = "user3Id";
        private const string User4IdCol = "user4Id";
        private const string User1ScoreCol = "user1Score";
        private const string User2ScoreCol = "user2Score";
        private const string User3ScoreCol = "user3Score";
        private const string User4ScoreCol = "user4Score";
        private const string PlatformCol = "platform";
        private const string Timestamp = "timestamp";
        private const string IdCol = "Id";
        private const string ServerIdCol = "serverId";
        private const string FullLogIdCol = "fullLog";
        private const string TimestampCol = "timestamp";
        

        public static (Game, List<Ranking>) RecordIRLGame(string[] members, Server server)
        {
            var dbCon = DBConnection.Instance();

            if (members[0] == members[1] || members[0] == members[2] || members[0] == members[3]
                || members[1] == members[2] || members[1] == members[3]
                || members[2] == members[3])
            {
                throw (new NotEnoughUsersException());
            }

            string logId = GetIRLLogId();

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {GameTableName} ({User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {IdCol}, {PlatformCol}, {ServerIdCol}) " +
                    $"VALUES ({members[0]}, {members[1]}, {members[2]}, {members[3]}, @logId, @gameType, @serverId);";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new SqlParameter("@logId", SqlDbType.NVarChar)
                {
                    Value = logId
                });
                command.Parameters.Add(new SqlParameter("@gameType", SqlDbType.NVarChar)
                {
                    Value = GameType.IRL
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.NVarChar)
                {
                    Value = server.Id
                });

                command.ExecuteNonQuery();

                var game = GetGameFromLogId(logId, server);
                var rankings = RankingDbService.UpdateRankings(game);

                return (game, rankings);
            }
            throw (new DbConnectionException());
        }

        public static (Game, List<Ranking>) RecordOnlineGame(RiichiGame gameLog, Server serverWithUsers)
        {
            var dbCon = DBConnection.Instance();

            var logId = gameLog.Ref;
            var fullLog = gameLog.FullLog;
            var gameType = gameLog.GameType;
            var playerNames = gameLog.Names;
            var playerMjIds = gameLog.UserIds; //mahjongsoul internal Ids
            var scores = gameLog.FinalScores;
            var userList = serverWithUsers.Users;
            var platform = gameLog.GameType;

            // TODO feed timestamp from game data
            if (playerNames.Length < 4)
            {
                throw new Exception("No sanma allowed");
            }

            var nameIdScorePos = new List<(string,string,int,int)>(); //0=start east, 1=start south,...
            for(int i=0; i<4; i++)
            {
                nameIdScorePos.Add((playerNames[i], playerMjIds[i], scores[i],i));
            }
            // We sort them by comparing their scores minus their initial pos, since initPos<<<score it's ok, I guess?
            nameIdScorePos.Sort((tuple1, tuple2) => (tuple2.Item3- tuple2.Item4).CompareTo((tuple1.Item3 - tuple1.Item4)));
            int[] sortedScores = nameIdScorePos.Select(x => x.Item3).ToArray();
            string[] sortedNames = nameIdScorePos.Select(x => x.Item1).ToArray();
            string[] sortedMjIds = nameIdScorePos.Select(x => x.Item2).ToArray();
            string[] sortedPlayerIds = new string[4];

            List<int> toRecordMjId = new List<int>();
            for(int i = 0; i<4; i++)
            {
                User user = null;
                switch (gameType)
                {
                    //Get the User objects from the Name or Ids present in the log
                    case GameType.Mahjsoul: 
                        user = userList.Find(x => x.MahjsoulUserId == sortedMjIds[i]);
                        if (user == null)
                        {
                            user = userList.Find(x => x.MahjsoulName == sortedNames[i]);
                            toRecordMjId.Add(i);
                        }
                        break;
                    case GameType.Tenhou:
                        user = userList.Find(x => x.TenhouName == sortedNames[i]);
                        break;
                    default:
                        throw (new Exception("Bad game type, can't record"));
                }
                if (user == null)
                {
                    throw (new Exception($"couldn't find player with this name on the server: {sortedNames[i]}"));
                }
                sortedPlayerIds[i] = user.Id;
            }

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {GameTableName} ({User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {IdCol}, {FullLogIdCol}, {PlatformCol}, {ServerIdCol}) " +
                    $"VALUES (@playerId1, @playerId2, @playerId3, @playerId4, {sortedScores[0]}, {sortedScores[1]}, {sortedScores[2]}, {sortedScores[3]}, @logId, @fullLog, @platform, @serverId);";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new SqlParameter("@playerId1", SqlDbType.NVarChar)
                {
                    Value = sortedPlayerIds[0]
                });
                command.Parameters.Add(new SqlParameter("@playerId2", SqlDbType.NVarChar)
                {
                    Value = sortedPlayerIds[1]
                });
                command.Parameters.Add(new SqlParameter("@playerId3", SqlDbType.NVarChar)
                {
                    Value = sortedPlayerIds[2]
                });
                command.Parameters.Add(new SqlParameter("@playerId4", SqlDbType.NVarChar)
                {
                    Value = sortedPlayerIds[3]
                });
                command.Parameters.Add(new SqlParameter("@fullLog", SqlDbType.NVarChar)
                {
                    Value = fullLog
                });
                command.Parameters.Add(new SqlParameter("@logId", SqlDbType.NVarChar)
                {
                    Value = logId
                });
                command.Parameters.Add(new SqlParameter("@platform", SqlDbType.NVarChar)
                {
                    Value = platform
                });
                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.NVarChar)
                {
                    Value = serverWithUsers.Id
                });
                command.ExecuteNonQuery();

                //Add the majsouldID to the new users:
                foreach(var idx in toRecordMjId)
                {
                    UserDbService.SetMahjsoulUserId(sortedPlayerIds[idx], sortedMjIds[idx]);
                }

                var game = GetGameFromLogId(logId, serverWithUsers);
                var rankings = RankingDbService.UpdateRankings(game);

                return (game, rankings);
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
                command.CommandText = $"SELECT {IdCol} FROM {GameTableName}";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                int nb = 0;
                while (Reader.Read())
                {
                    nb++;
                }
                Reader.Close();
                return nb;
            }
            throw (new DbConnectionException());
        }

        private static string GetIRLLogId()
        {
            string maxId = $"{GetMaxId()}".PadLeft(4,'0');
            maxId = maxId.Substring(maxId.Length - 3);
            return DateTime.Now.ToString($"yyyyMMdd-HHmm-"+maxId);
        }

        public static Game GetLastRecordedGame(Server server)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {IdCol}, {User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {FullLogIdCol}, {PlatformCol}, {TimestampCol}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {ServerIdCol} = @serverId" +
                    $" ORDER BY {Timestamp} DESC";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new SqlParameter("@serverId", SqlDbType.VarChar)
                {
                    Value = server.Id
                });

                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    string id = Reader.GetString(0);
                    string user1Id = Reader.GetString(1);
                    string user2Id = Reader.GetString(2);
                    string user3Id = Reader.GetString(3);
                    string user4Id = Reader.GetString(4);
                    int user1Score = Reader.IsDBNull(5) ? 0 : Reader.GetInt32(5);
                    int user2Score = Reader.IsDBNull(6) ? 0 : Reader.GetInt32(6);
                    int user3Score = Reader.IsDBNull(7) ? 0 : Reader.GetInt32(7);
                    int user4Score = Reader.IsDBNull(8) ? 0 : Reader.GetInt32(8);
                    string fullLog = Reader.IsDBNull(9) ? null : Reader.GetString(9);
                    string platformStr = Reader.GetString(10);
                    DateTime timestamp = Reader.GetDateTime(11);
                    GameType platform = (GameType)Enum.Parse(typeof(GameType), platformStr);

                    Reader.Close();
                    var game = new Game(id, server, user1Id, user2Id, user3Id, user4Id, platform, timestamp);
                    game.FullLog = fullLog;
                    game.User1Score = user1Score;
                    game.User2Score = user2Score;
                    game.User3Score = user3Score;
                    game.User4Score = user4Score;
                }
            }
            throw (new DbConnectionException());
        }

        public static Game GetGameFromLogId(string logId, Server server)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {ServerIdCol}, {PlatformCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {TimestampCol}, {FullLogIdCol}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {IdCol} = @logId";

                command.Parameters.Add(new SqlParameter("@logId", SqlDbType.VarChar)
                {
                    Value = logId
                });

                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    string user1Id = Reader.GetString(0);
                    string user2Id = Reader.GetString(1);
                    string user3Id = Reader.GetString(2);
                    string user4Id = Reader.GetString(3);
                    string serverId = Reader.GetString(4);
                    GameType platform = (GameType)Enum.Parse(typeof(GameType), Reader.GetString(5));
                    int user1Score = Reader.IsDBNull(6) ? 0 : Reader.GetInt32(6);
                    int user2Score = Reader.IsDBNull(7) ? 0 : Reader.GetInt32(7);
                    int user3Score = Reader.IsDBNull(8) ? 0 : Reader.GetInt32(8);
                    int user4Score = Reader.IsDBNull(9) ? 0 : Reader.GetInt32(9);
                    DateTime timestamp = Reader.GetDateTime(10);
                    string fullLog = Reader.IsDBNull(11) ? null : Reader.GetString(11);

                    Reader.Close();
                    if (serverId != server.Id)
                    {
                        throw new GetGameException($"Game with logId:{logId} has been recorded on another server");
                    }
                    var game = new Game(logId, server, user1Id, user2Id, user3Id, user4Id, platform, timestamp);
                    game.User1Score = user1Score;
                    game.User2Score = user2Score;
                    game.User3Score = user3Score;
                    game.User4Score = user4Score;
                    game.FullLog = fullLog;
                    return game;

                }
                throw (new GetGameException($"Game with logId:{logId} was not found"));
            }
            throw (new DbConnectionException());
        }
    }
}
