using kandora.bot.exceptions;
using kandora.bot.http;
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
    class ScoreDbService : DbService
    {
        private const string GameTableName = "Game";

        //Game
        private const string User1IdCol = "user1Id";
        private const string User2IdCol = "user2Id";
        private const string User3IdCol = "user3Id";
        private const string User4IdCol = "user4Id";
        private const string User1ScoreCol = "user1Score";
        private const string User2ScoreCol = "user2Score";
        private const string User3ScoreCol = "user3Score";
        private const string User4ScoreCol = "user4Score";
        private const string User1ChomboCol = "user1Chombo";
        private const string User2ChomboCol = "user2Chombo";
        private const string User3ChomboCol = "user3Chombo";
        private const string User4ChomboCol = "user4Chombo";
        private const string PlatformCol = "platform";
        private const string LocationCol = "location";
        private const string TimestampCol = "timestamp";
        private const string IdCol = "Id";
        private const string ServerIdCol = "serverId";
        private const string FullLogIdCol = "fullLog";
        private const string NameCol = "name";
        private const string IsSanmaCol = "isSanma"; 


        public static (Game, List<Ranking>) RecordIRLGame(string[] members, int[] scores, int[] chombos, DateTime timeStamp, string location, Server server, LeagueConfig leagueConfig)
        {
            var dbCon = DBConnection.Instance();

            string logId = GetIRLLogName(server.Id, location);

            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;

                var scoreColumns = scores == null
                    ? ""
                    : $", {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}{(scores.Length > 3 ? $", {User4ScoreCol}" : "")}";

                var scoreValues = scores == null
                    ? ""
                    : $", @score1, @score2, @score3{(scores.Length > 3 ? $", @score4" : "")}";

                command.CommandText = $"INSERT INTO {GameTableName} ({NameCol}, {User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {PlatformCol}, {IsSanmaCol}, {ServerIdCol}, {TimestampCol}{scoreColumns}, {User1ChomboCol}, {User2ChomboCol}, {User3ChomboCol}, {User4ChomboCol}) " +
                    $"VALUES (@gameName, \'{members[0]}\', \'{members[1]}\', \'{members[2]}\', \'{members[3]}\', @gameType, @isSanma, @serverId, @timestamp{scoreValues}, {chombos[0]}, {chombos[1]}, {chombos[2]}, {chombos[3]});";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@gameName", NpgsqlDbType.Varchar, logId);
                command.Parameters.AddWithValue("@gameType", NpgsqlDbType.Varchar, GameType.IRL.ToString());
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, server.Id);
                command.Parameters.AddWithValue("@isSanma", NpgsqlDbType.Boolean, members.Length == 3);
                command.Parameters.AddWithValue("@timestamp", NpgsqlDbType.Timestamp, timeStamp);
                if (scores != null)
                {
                    for (int i = 0; i < scores.Length && i < 4; i++)
                    {
                        command.Parameters.AddWithValue($"@score{i + 1}", NpgsqlDbType.Integer, scores[i]);
                    }
                }
                command.ExecuteNonQuery();

                //var game = GetGameFromGameName(logId, server);

                //only take game into account if it's between the league timePeriod
                if (timeStamp.CompareTo(leagueConfig.StartTime) > 0
                    && timeStamp.CompareTo(leagueConfig.EndTime) < 0)
                {
                    //var rankings = RankingDbService.UpdateRankings(game, leagueConfig);
                    //return (game, rankings);
                }
                else
                {
                    //var rankings = RankingDbService.GetServerRankings(server.Id);
                    //return (game, rankings);
                }
                return (null, new List<Ranking>());

            }
            throw (new DbConnectionException());
        }

        public static (Game, List<Ranking>) RecordOnlineGame(RiichiGame gameLog, Server serverWithUsers, LeagueConfig config)
        {
            var dbCon = DBConnection.Instance();

            var logId = gameLog.Ref;
            var fullLog = gameLog.FullLog;
            var gameType = gameLog.GameType;
            var playerNames = gameLog.Names;
            var playerMjIds = gameLog.UserIds; //mahjongsoul internal Ids
            var scores = gameLog.FinalScores;
            var userList = serverWithUsers.Users;
            var platform = gameLog.GameTypeStr;
            var timestamp = gameLog.Timestamp;

            // TODO feed timestamp from game data
            if (playerNames.Length < 3)
            {
                throw new Exception("Not enough players");
            }

            var nameIdScorePos = new List<(string, string, int, int)>(); //0=start east, 1=start south,...
            for (int i = 0; i < playerNames.Length; i++)
            {
                nameIdScorePos.Add((playerNames[i], playerMjIds[i], scores[i], i));
            }
            // We sort them by comparing their scores minus their initial pos, since initPos<<<score it's ok, I guess?
            nameIdScorePos.Sort((tuple1, tuple2) => (tuple2.Item3 - tuple2.Item4).CompareTo((tuple1.Item3 - tuple1.Item4)));

            var playerIds = new string[playerNames.Length];

            List<int> toRecordMjId = new List<int>();
            for (int i = 0; i < playerNames.Length; i++)
            {
                User user = null;
                switch (gameType)
                {
                    //Get the User objects from the Name or Ids present in the log
                    case GameType.Mahjsoul:
                        user = userList.Find(x => x.MahjsoulUserId == playerMjIds[i]);
                        if (user == null)
                        {
                            user = userList.Find(x => x.MahjsoulName == playerNames[i]);
                            toRecordMjId.Add(i);
                        }
                        break;
                    case GameType.Tenhou:
                        user = userList.Find(x => x.TenhouName == playerNames[i]);
                        break;
                    default:
                        throw (new Exception("Bad game type, can't record"));
                }
                if (user == null)
                {
                    throw (new Exception($"couldn't find player with this name on the server: {playerNames[i]}"));
                }
                playerIds[i] = user.Id;
            }

            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"INSERT INTO {GameTableName} ({User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {NameCol}, {FullLogIdCol}, {PlatformCol}, {ServerIdCol}, {IsSanmaCol}, {TimestampCol}) " +
                    $"VALUES (@playerId1, @playerId2, @playerId3, @playerId4, {scores[0]}, {scores[1]}, {scores[2]}, {scores[3]}, @gameName, @fullLog, @platform, @serverId, @isSanma, @timestamp);";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@playerId1", NpgsqlDbType.Varchar, playerIds[0]);
                command.Parameters.AddWithValue("@playerId2", NpgsqlDbType.Varchar, playerIds[1]);
                command.Parameters.AddWithValue("@playerId3", NpgsqlDbType.Varchar, playerIds[2]);
                command.Parameters.AddWithValue("@playerId4", NpgsqlDbType.Varchar, playerIds[3]);
                command.Parameters.AddWithValue("@fullLog", NpgsqlDbType.Varchar, fullLog);
                command.Parameters.AddWithValue("@gameName", NpgsqlDbType.Varchar, logId);
                command.Parameters.AddWithValue("@platform", NpgsqlDbType.Varchar, platform);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverWithUsers.Id);
                command.Parameters.AddWithValue("@isSanma", NpgsqlDbType.Boolean, playerIds.Length == 3);
                command.Parameters.AddWithValue("@timestamp", NpgsqlDbType.Timestamp, timestamp);
                command.ExecuteNonQuery();

                //Add the majsouldID to the new users:
                foreach (var idx in toRecordMjId)
                {
                    UserDbService.SetMahjsoulUserId(playerIds[idx], playerMjIds[idx]);
                }
                var game = GetGameFromGameName(logId, serverWithUsers);


                //only take game into account if it's between the league timePeriod
                if (game.Timestamp.CompareTo(config.StartTime) > 0
                    && game.Timestamp.CompareTo(config.EndTime) < 0)
                {
                    var rankings = RankingDbService.UpdateRankings(game, config);
                    return (game, rankings);
                }
                else
                {
                    var rankings = RankingDbService.GetServerRankings(serverWithUsers.Id);
                    return (game, rankings);
                }
            }
            throw (new DbConnectionException());
        }


        protected static string GetIRLLogName(string serverId, string locationParam)
        {
            var location = locationParam.Length == 0 ? "IRLGame" : locationParam;

            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {LocationCol} FROM {GameTableName} WHERE {ServerIdCol} = @serverId AND {LocationCol} = @location ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@location", NpgsqlDbType.Varchar, location);
                Reader = command.ExecuteReader();
                int nb = 0;
                while (Reader.Read())
                {
                    nb++;
                }
                var index = $"{nb}".PadLeft(4, '0');
                Reader.Close();
                return $"{location}-{index}";
            }
            throw (new DbConnectionException());
        }

        public static void DeleteGamesFromServer(string serverId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"DELETE FROM {GameTableName} WHERE {ServerIdCol} = \'{serverId}\';";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        public static List<Game> GetLastNRecordedGame(Server server, LeagueConfig config, int numberOfGames = -1)
        {
            var dbCon = DBConnection.Instance();
            var games = new List<Game>();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {IdCol}, {User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {FullLogIdCol}, {PlatformCol}, {TimestampCol}, {IsSanmaCol}, {NameCol}, {LocationCol}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {ServerIdCol} = @serverId" +
                    (config == null ? "" : $" AND {TimestampCol} > @startTime AND {TimestampCol} < @endTime") +
                    $" ORDER BY {TimestampCol} DESC, {IdCol} DESC;";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, server.Id);
                if (config != null)
                {
                    command.Parameters.AddWithValue("@startTime", NpgsqlDbType.Timestamp, config.StartTime);
                    command.Parameters.AddWithValue("@endTime", NpgsqlDbType.Timestamp, config.EndTime);
                }

                Reader = command.ExecuteReader();
                var idx = 0;
                while (Reader.Read() && (numberOfGames < 0 || idx < numberOfGames))
                {
                    int id = Reader.GetInt32(0);
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
                    bool isSanma = Reader.GetBoolean(12);
                    string name = Reader.GetString(13);
                    string location = Reader.GetString(14);
                    GameType platform = (GameType)Enum.Parse(typeof(GameType), platformStr);

                    var game = new Game(id, name, server, user1Id, user2Id, user3Id, user4Id, platform, location, timestamp, isSanma);
                    game.FullLog = fullLog;
                    game.User1Score = user1Score;
                    game.User2Score = user2Score;
                    game.User3Score = user3Score;
                    game.User4Score = user4Score;

                    games = games.Append(game).ToList();
                    idx++;
                }

                Reader.Close();
            }
            return games;
            throw (new DbConnectionException());
        }

        public static bool DoesGameExist(string gameName, Server server)
        {
            try
            {
                var game = GetGameFromGameName(gameName, server);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Game GetGameFromGameName(string gameName, Server server)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {User1IdCol}, {User2IdCol}, {User3IdCol}, {User4IdCol}, {ServerIdCol}, {PlatformCol}, {User1ScoreCol}, {User2ScoreCol}, {User3ScoreCol}, {User4ScoreCol}, {TimestampCol}, {FullLogIdCol}, {IsSanmaCol}, {User1ChomboCol}, {User2ChomboCol}, {User3ChomboCol}, {User4ChomboCol}, {IdCol}, {LocationCol}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {NameCol} = @name AND {ServerIdCol} = @serverId";

                command.Parameters.AddWithValue("@name", NpgsqlDbType.Varchar, gameName);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, server.Id);

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
                    bool isSanma = Reader.IsDBNull(12) ? false : Reader.GetBoolean(12);

                    int user1Chombo = Reader.IsDBNull(13) ? 0 : Reader.GetInt32(13);
                    int user2Chombo = Reader.IsDBNull(14) ? 0 : Reader.GetInt32(14);
                    int user3Chombo = Reader.IsDBNull(15) ? 0 : Reader.GetInt32(15);
                    int user4Chombo = Reader.IsDBNull(16) ? 0 : Reader.GetInt32(16);

                    int gameId = Reader.GetInt32(16);
                    string location = Reader.GetString(17);

                    Reader.Close();

                    var game = new Game(gameId, gameName, server, user1Id, user2Id, user3Id, user4Id, platform, location, timestamp, isSanma);
                    game.User1Score = user1Score;
                    game.User2Score = user2Score;
                    game.User3Score = user3Score;
                    game.User4Score = user4Score;
                    game.User1Chombo = user1Chombo;
                    game.User2Chombo = user2Chombo;
                    game.User3Chombo = user3Chombo;
                    game.User4Chombo = user4Chombo;
                    game.FullLog = fullLog;
                    return game;
                }
                Reader.Close();
                throw (new Exception($"Game with gameName:{gameName} was not found on this league"));
            }
            throw (new DbConnectionException());
        }
    }
}
