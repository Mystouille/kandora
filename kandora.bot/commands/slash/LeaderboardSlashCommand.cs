using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.exceptions;
using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.discord;
using kandora.bot.services.http;
using kandora.bot.utils;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("leaderboard", Resources.leaderboard_groupDescription, defaultPermission: false)]
    class LeaderboardSlashCommands : KandoraSlashCommandModule
    {
        [SlashCommand("register", Resources.leaderboard_register_description)]
        public async Task Register(InteractionContext ctx,
             [Option(Resources.leaderboard_register_mahjsoulName, Resources.leaderboard_register_mahjsoulName_description)] string mahjsoulName = "",
             [Option(Resources.leaderboard_register_mahjsoulFriendId, Resources.leaderboard_register_mahjsoulFriendId_description)] string mahjsoulFriendId = "",
             [Option(Resources.leaderboard_register_tenhouName, Resources.leaderboard_register_tenhouName_description)] string tenhouName = "",
             [Option(Resources.leaderboard_register_riichiCityId, Resources.leaderboard_register_riichiCityId_description)] long riichiCityId = -1,
             [Option(Resources.leaderboard_register_riichiCityName, Resources.leaderboard_register_riichiCityName_description)] string riichiCityName = "")
        {
            try
            {
                var serverId = ctx.Guild.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                var userId = ctx.User.Id.ToString();


                var discordUsers = await ctx.Guild.GetAllMembersAsync();
                var matchingUsers = discordUsers.Where(x=> x.Id.ToString() == userId).ToList();
                var userName = matchingUsers.Count > 0 ? matchingUsers[0].DisplayName : null;

                if (mahjsoulName.Length > 0)
                {
                    if(UserDbService.MahjsoulNameExistAlready(userId, serverId, mahjsoulName))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_mahjsoulName, mahjsoulName));
                    }
                }
                if (mahjsoulFriendId.Length > 0)
                {
                    if (UserDbService.MahjsoulFriendIdExistAlready(userId, serverId, mahjsoulFriendId))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_mahjsoulFriendId, mahjsoulFriendId));
                    }
                }
                if (tenhouName.Length > 0)
                {
                    if (UserDbService.TenhouNameExistAlready(userId, serverId, tenhouName))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_tenhouName, tenhouName));
                    }
                }
                if (tenhouName.Length > 0)
                {
                    if (UserDbService.TenhouNameExistAlready(userId, serverId, tenhouName))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_tenhouName, tenhouName));
                    }
                }
                if (riichiCityId >= 0)
                {
                    if (UserDbService.RiichiCityIdExistAlready(userId, serverId, (int)riichiCityId))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_riichiCityId, riichiCityId));
                    }
                }
                if (riichiCityName.Length > 0)
                {
                    if (UserDbService.RiichiCityNameExistAlready(userId, serverId, riichiCityName))
                    {
                        throw new Exception(String.Format(Resources.commandError_ValueAlreadyExists, Resources.leaderboard_register_riichiCityName, riichiCityName));
                    }
                }

                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var configId = (int)(server.LeaderboardConfigId);
                var config = ConfigDbService.GetConfig(configId);
                var responseMessage = "";
                if (ServerDbService.IsUserInServer(userId, serverId))
                {
                    if(RankingDbService.GetUserRankingHistory(userId, serverId, latest: true).Count == 0)
                    {
                        RankingDbService.InitUserRanking(userId, serverId, config);
                        responseMessage = Resources.leaderboard_register_response_newRanking;
                    }
                    else
                    {
                        responseMessage = Resources.leaderboard_register_response_userAlreadyRegistered;
                    }
                }
                else
                {
                    UserDbService.CreateUser(ctx.User.Id.ToString(), userName, serverId, config);
                    RankingDbService.InitUserRanking(userId, serverId, config);
                    responseMessage = Resources.leaderboard_register_response_newUser;
                }

                if (mahjsoulName.Length > 0)
                {
                    UserDbService.SetMahjsoulName(userId, mahjsoulName);
                }
                if (mahjsoulFriendId.Length > 0)
                {
                    UserDbService.SetMahjsoulFriendId(userId, mahjsoulFriendId);
                }
                if (tenhouName.Length > 0)
                {
                    UserDbService.SetTenhouName(userId, tenhouName);
                }
                if (riichiCityId >= 0)
                {
                    UserDbService.SetRiichiCityId(userId, (int)riichiCityId);
                }
                if (riichiCityName.Length > 0)
                {
                    UserDbService.SetRiichiCityName(userId, riichiCityName);
                }
                var rb = new DiscordInteractionResponseBuilder().WithContent(responseMessage).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);

            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeLog", Resources.leaderboard_logInfo_description)]
        public async Task SeeLog(InteractionContext ctx,
             [Option(Resources.leaderboard_option_gameId, Resources.leaderboard_option_gameId_description)] string gameId)
        {
            try
            {
                var log = await LogService.Instance.GetLog(gameId, 2);
                var gameResult = PrintGameResult(log, ctx.Client);

                var rb = new DiscordInteractionResponseBuilder().WithContent(gameResult).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("getGames", Resources.leaderboard_getGames_description)]
        public async Task GetGames(InteractionContext ctx)
        {
            try
            {

                var serverId = ctx.Guild.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                var userId = ctx.User.Id.ToString();

                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var configId = (int)(server.LeaderboardConfigId);
                var config = ConfigDbService.GetConfig(configId);
                var logs = ScoreDbService.GetLastNRecordedGame(serverId, config);
                logs.Reverse();

                var sb = new StringBuilder();
                var userNameCache = new Dictionary<string, string>();
                sb.AppendLine(Resources.leaderboard_getGames_fileHeader);
                
                foreach (var log in logs)
                {

                    sb.AppendLine($"{log.Id},{log.GameName},{log.LocationStr},{log.Timestamp.ToString("yyyy/MM/dd HH:mm")}," +
                        $"{getPlayerSimpleName(log.User1Id,ctx,userNameCache)},{log.User1Score},{log.User1Chombo}," +
                        $"{getPlayerSimpleName(log.User2Id, ctx, userNameCache)},{log.User2Score},{log.User2Chombo}," +
                        $"{getPlayerSimpleName(log.User3Id, ctx, userNameCache)},{log.User3Score},{log.User3Chombo}," +
                        $"{getPlayerSimpleName(log.User4Id, ctx, userNameCache)},{log.User4Score},{log.User4Chombo}");
                }
                var startTime = config.StartTime.ToString("yyyy/MM/dd");
                var endTime = config.EndTime.ToString("yyyy/MM/dd");
                var leaderboardTime = $"{startTime}-{endTime}";
                var rb = new DiscordInteractionResponseBuilder()
                    .AddFile($"{ctx.Guild.Name}leaderboard{leaderboardTime}.csv",FileUtils.GenerateStreamFromString(sb.ToString()))
                    .WithContent(String.Format(Resources.leaderboard_getGames_message, startTime, endTime))
                    .AsEphemeral(); ;
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }


        [SlashCommand("submit", Resources.leaderboard_submitResult_description)]
        public async Task SubmitIRL(InteractionContext ctx,
            [Option(Resources.leaderboard_option_player1, Resources.leaderboard_option_anyPlayer_description)] string player1,
            [Option(Resources.leaderboard_option_player1Score, Resources.leaderboard_option_anyScore_description)] string score1,
            [Option(Resources.leaderboard_option_player2, Resources.leaderboard_option_anyPlayer_description)] string player2,
            [Option(Resources.leaderboard_option_player2Score, Resources.leaderboard_option_anyScore_description)] string score2,
            [Option(Resources.leaderboard_option_player3, Resources.leaderboard_option_anyPlayer_description)] string player3,
            [Option(Resources.leaderboard_option_player3Score, Resources.leaderboard_option_anyScore_description)] string score3,
            [Option(Resources.leaderboard_option_player4, Resources.leaderboard_option_anyPlayer_description)] string player4,
            [Option(Resources.leaderboard_option_player4Score, Resources.leaderboard_option_anyScore_description)] string score4,
            [Option(Resources.leaderboard_option_player1Chombo, Resources.leaderboard_option_chombo1_description)] long chombo1 = 0,
            [Option(Resources.leaderboard_option_player2Chombo, Resources.leaderboard_option_chombo2_description)] long chombo2 = 0,
            [Option(Resources.leaderboard_option_player3Chombo, Resources.leaderboard_option_chombo3_description)] long chombo3 = 0,
            [Option(Resources.leaderboard_option_player4Chombo, Resources.leaderboard_option_chombo4_description)] long chombo4 = 0,
            [Option(Resources.leaderboard_option_date, Resources.leaderboard_option_date_description)] string date = "",
            [Option(Resources.leaderboard_option_location, Resources.leaderboard_option_location_description)] string location = ""
            )
        {
            try
            {
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var serverId = ctx.Guild.Id.ToString();
                var server = servers[serverId];
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var leaderboardConfig = ConfigDbService.GetConfig((int)(server.LeaderboardConfigId));


                var player1Id = getIdFromPlayerParam(player1);
                var player2Id = getIdFromPlayerParam(player2);
                var player3Id = getIdFromPlayerParam(player3);
                var player4Id = getIdFromPlayerParam(player4);

                var timestamp = date.Length == 0 ? DateTime.Now : DateTime.ParseExact(date, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);



                var usersScores = new (string, string, long, string)[] { (player1Id, score1, chombo1, player1), (player2Id, score2, chombo2, player2), (player3Id, score3, chombo3, player3), (player4Id, score4, chombo4, player4) };

                var sortedScores = usersScores.ToList();
                sortedScores.Sort((tuple1, tuple2) => tuple2.Item2.CompareTo(tuple1.Item2));

                var userIds = sortedScores.Select(x => x.Item1).ToArray();
                var scoresStr = sortedScores.Select(x => x.Item2).ToArray();
                var chombos = sortedScores.Select(x => (int)(x.Item3)).ToArray();
                var playerNameInput = sortedScores.Select(x => x.Item4).ToArray();
                int[] scores = null;
                if (scoresStr != null)
                {
                    var tempScores = scoresStr.Select(x => float.Parse(x)).ToArray();
                    // is scores are 32.3, 27.7, etc, convert them to 32300, ...
                    if (tempScores.All(x => x < 1000))
                    {
                        scores = tempScores.Select(x => (int)(x * 1000)).ToArray();
                    }
                    // is scores are 32300, 27700, etc, let them as is
                    else
                    {
                        scores = tempScores.Select(x => (int)(x)).ToArray();
                    }
                }
                foreach (var tuple in usersScores)
                {
                    if (server.Users.Where(x => x.Id == tuple.Item1).Count() == 0)
                    {
                        throw new Exception($"{String.Format(Resources.commandError_PlayerUnknown, tuple.Item4)}");
                    }
                }
                var scoreSum = scores.Sum();
                var targetScoreSum = leaderboardConfig.StartingPoints * 1000 * 4;
                if (scoreSum != targetScoreSum)
                {
                    
                    throw new Exception(String.Format(Resources.commandError_Wrong_Scores, scoreSum, targetScoreSum));
                }
                if (leaderboardConfig.CountPoints && scores == null)
                {
                    throw new Exception(Resources.commandError_LeaderboardConfigRequiresScore);
                }
                if (!leaderboardConfig.AllowSanma && userIds.Length == 3)
                {
                    throw new Exception(Resources.commandError_sanmaNotAllowed);
                }
                if (userIds.Length < 3 || userIds.Length > 4)
                {
                    throw new Exception(String.Format(Resources.commandError_badPlayerNumber, userIds.Length));
                }
                var distinctUsers = userIds.Distinct();
                if (distinctUsers.Count() < userIds.Length)
                {
                    throw new Exception(String.Format(Resources.commandError_badDistinctPlayerNumber, distinctUsers.Count()));
                }


                var dummyGame = new Game(0, "", serverId, userIds[0], userIds[1], userIds[2], userIds[3], GameType.IRL, "nowhere", timestamp, isSanma: false);
                dummyGame.User1Score = scores[0];
                dummyGame.User2Score = scores[1];
                dummyGame.User3Score = scores[2];
                dummyGame.User4Score = scores[3];
                dummyGame.User1Chombo = chombos[0];
                dummyGame.User2Chombo = chombos[1];
                dummyGame.User3Chombo = chombos[2];
                dummyGame.User4Chombo = chombos[3];

                var similarGames = ScoreDbService.CheckGame(dummyGame, serverId);

                var gameResult = PrintGameResult(dummyGame, similarGames, server);

                var gameMsg = $"{Resources.leaderboard_submitResult_voteMessage}\n{gameResult}";
                await kandoraContext.AddPendingGame(ctx, gameMsg, new PendingGame(userIds, scores, chombos, location, timestamp, server));
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string getIdFromPlayerParam(string playerStr)
        {
            if (playerStr.StartsWith("<@") && playerStr.EndsWith(">"))
            {
                return playerStr.Substring(2, playerStr.Length - 3);
            }
            return playerStr;
        }

        private static string getPlayerMention(string playerId)
        {
            if (long.TryParse(playerId, out _))
            {
                return $"<@{playerId}>";
            }
            return playerId;
        }

        private static string getPlayerSimpleName(string playerId, InteractionContext ctx, Dictionary<string,string> userNameCache)
        {
            if (long.TryParse(playerId, out _))
            {
                var key = Convert.ToUInt64(playerId);
                if (ctx.Guild.Members.ContainsKey(key))
                {
                    DiscordMember discordMember;
                    ctx.Guild.Members.TryGetValue(key, out discordMember);
                    return discordMember.DisplayName;
                }
            }
            return playerId;
        }

        [SlashCommand("submitLog", Resources.leaderboard_submitOnlineResult_description)]
        public async Task Submit(InteractionContext ctx,
            [Option(Resources.leaderboard_submitOnlineResult_gameId, Resources.leaderboard_submitOnlineResult_gameId_description)] string gameId)
        {
            try
            {
                var serverId = ctx.Guild.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                var serverUsers = server.Users;
                var log = await LogService.Instance.GetLog(gameId, 2);

                var gameExists = ScoreDbService.DoesGameExist(log.Ref, server);

                if (gameExists)
                {
                    var rb = new DiscordInteractionResponseBuilder().WithContent(String.Format(Resources.commandError_gameAlreadyExists, log.Ref)).AsEphemeral();
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
                    return;
                }

                var users = await GetUsersFromLog(log, serverUsers, ctx);
                var gameResult = PrintGameResult(log, ctx.Client, users);

                var gameMsg = $"{Resources.leaderboard_submitResult_voteMessage}\n{gameResult}";
                await kandoraContext.AddPendingGame(ctx, gameMsg, new PendingGame(users.Select(x => x.Id.ToString()).ToArray(), server, log));
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeRanking", Resources.leaderboard_seeRanking_description)]
        public async Task SeeRanking(InteractionContext ctx,
            [Option(Resources.leaderboard_seeRanking_minGames, Resources.leaderboard_seeRanking_minGames_description)] long nbMin = 10)
        {
            try
            {
                var userId = ctx.User.Id.ToString();
                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var server = ServerDbService.GetServer(serverId);
                var configId = server.LeaderboardConfigId;
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var config = ConfigDbService.GetConfig((int)(server.LeaderboardConfigId));
                List<Ranking> rankingList = RankingDbService.GetServerRankings(serverId);
                var latestUserRankings = new Dictionary<string, (Ranking, int)>();
                var checkList = new HashSet<string>();

                foreach (var rank in rankingList)
                {
                    var id = rank.UserId;
                    if (!checkList.Contains(id))
                    {
                        var history = RankingDbService.GetUserRankingHistory(id, serverId);
                        if (history.Count() - 1 > nbMin)
                        {
                            latestUserRankings.Add(id, (rank, history.Count() -1));
                        }
                        checkList.Add(id);
                    }
                }

                List<(Ranking,int)> sortedRanks = latestUserRankings.Values.ToList();
                sortedRanks.Sort((val1, val2) => val2.Item1.NewRank.CompareTo(val1.Item1.NewRank));
                StringBuilder sb = new StringBuilder();
                int i = 1;
                sb.AppendLine(":partying_face:");
                foreach (var rank in sortedRanks)
                {
                    var userName = ctx.Channel.Users.Where(x => x.Id.ToString() == rank.Item1.UserId).Select(x => x.Nickname).FirstOrDefault();
                    var userStr = userName != null ? userName : rank.Item1.UserId;
                    var nbGames = rank.Item2;
                       
                    long parseResult;
                    if (!long.TryParse(rank.Item1.UserId, out parseResult))
                    {
                        userStr = rank.Item1.UserId;
                    }

                    var rankValue = Math.Round(rank.Item1.NewRank/1000,2);
                    if (config.EloSystem != "Average")
                    {
                        Convert.ToInt32(rank.Item1.NewRank);
                    }
                    sb.Append($"{i}: {getPlayerMention(rank.Item1.UserId)}:\t{rankValue}\t({rank.Item2}){(rank.Item1.UserId == userId ? $"<<< {Resources.leaderboard_seeRanking_youAreHere}" : "")}\n");
                    i++;
                }
                var leaderboard = sb.ToString();
                var rb = new DiscordInteractionResponseBuilder().WithContent(leaderboard).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeLastGames", Resources.leaderboard_seeLastGames_description)]
        public async Task SeeLastGames(InteractionContext ctx)
        {
            var serverId = ctx.Guild.Id.ToString();
            var server = ServerDbService.GetServer(serverId);

            if (server.LeaderboardConfigId == null)
            {
                throw new Exception(Resources.commandError_leaderboardNotInitialized);
            }
            var config = ConfigDbService.GetConfig((int)(server.LeaderboardConfigId));
            var games = ScoreDbService.GetLastNRecordedGame(serverId, config, 10);

            StringBuilder sb = new StringBuilder();
            foreach (Game game in games)
            {
                List<(string, int?)> userPlacements = new List<(string, int?)>();
                userPlacements.Add((game.User1Id, game.User1Score));
                userPlacements.Add((game.User2Id, game.User2Score));
                userPlacements.Add((game.User3Id, game.User3Score));
                userPlacements.Add((game.User4Id, game.User4Score));
                userPlacements.Sort();
                sb.Append($"{game.Id}\t{game.Platform}\t{game.Timestamp}\t");
                foreach (var placement in userPlacements)
                {
                    string playerId = placement.Item1;
                    bool isInt = Int64.TryParse(placement.Item1, out _);
                    if (isInt)
                    {
                        playerId = $"<@{placement.Item1}>";
                    }
                    sb.Append($"{playerId}: {placement.Item2} / ");
                }
                sb.AppendLine();
            }
            if(games.Count == 0)
            {
                sb.AppendLine(String.Format(Resources.leaderboard_seeLastGames_noGames,config.StartTime.ToString("yyyy/MM/dd"), config.EndTime.ToString("yyyy/MM/dd")));
            }
            var rb = new DiscordInteractionResponseBuilder().WithContent(sb.ToString()).AsEphemeral();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
        }

        //Checks if a log has all its player in the leaderboard and return them
        private async Task<List<User>> GetUsersFromLog(RiichiGame log, List<User> users, InteractionContext ctx)
        {
            int nbPlayers = log.Names.Length;
            List<string> notFound = new List<string>();
            List<User> foundUsers = new List<User>();
            for (int i = 0; i < nbPlayers; i++)
            {
                User foundUser = null;
                foreach (var user in users)
                {
                    switch (log.GameType)
                    {
                        case GameType.Mahjsoul:
                            bool matchName = user.MahjsoulName != null && user.MahjsoulName == log.Names[i];
                            bool matchId = user.MahjsoulUserId != null && user.MahjsoulUserId == log.UserIds[i];
                            if (matchId && !matchName)
                            {
                                UserDbService.SetMahjsoulName(user.Id, log.Names[i]);
                                var notification = String.Format(Resources.commandError_mahjsoulUserNameChanged, user.Id, user.MahjsoulName, log.Names[i]);
                                var rb = new DiscordInteractionResponseBuilder().WithContent(notification);
                                await ctx.Channel.SendMessageAsync(notification).ConfigureAwait(true);
                            }
                            if (matchName && !matchId)
                            {
                                UserDbService.SetMahjsoulUserId(user.Id, log.UserIds[i]);
                            }
                            if (matchName || matchId)
                            {
                                foundUser = user;
                            }
                            break;
                        case GameType.Tenhou:
                            if (user.TenhouName != null && user.TenhouName == log.Names[i])
                            {
                                foundUser = user;
                            }
                            break;
                        default:
                            throw new Exception(Resources.commandError_unknownLogFormat);
                    }
                    if (foundUser != null)
                    {
                        break;
                    }
                }
                if (foundUser == null)
                {
                    notFound.Add(log.Names[i]);
                }
                else
                {
                    foundUsers.Add(foundUser);
                }
            }
            if (notFound.Count > 0)
            {
                {
                    throw new Exception(String.Format(Resources.commandError_unknownOnlinePlayerName, string.Join(", ", notFound)));
                }
            }
            return foundUsers;
        }

        private static string PrintGameResult(RiichiGame game, DiscordClient client, List<User> users = null)
        {
            StringBuilder sb = new StringBuilder();
            if (game.Title != null)
            {
                sb.Append($"{Resources.leaderboard_title}: {game.Title[0]}\n");
            }
            sb.Append($"{Resources.leaderboard_date}: {game.Timestamp.ToString("yyyy/MM/dd HH:mm")}\n");
            sb.Append($"{Resources.leaderboard_results}: \n");
            var names = game.Names;
            var discordIds = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                //Also get the discord user's mention
                if (users != null)
                {
                    User user = null;
                    if (game.GameType == GameType.Tenhou)
                    {
                        user = users.Find(x => x.TenhouName == name);
                    }
                    else if (game.GameType == GameType.Mahjsoul)
                    {
                        user = users.Find(x => x.MahjsoulName == name);
                        if (user == null)
                        {
                            user = users.Find(x => x.MahjsoulUserId != null && x.MahjsoulUserId == game.UserIds[i]);
                        }
                    }
                    if (user != null)
                    {
                        discordIds[i] = $"<@{user.Id}>";
                    }
                    else
                    {
                        throw new Exception($"{String.Format(Resources.commandError_CouldNotFindGameUser, game.GameTypeStr, name)}");
                    }
                }
                sb.Append($"{discordIds[i]}({name}):\t{game.FinalScores[i]}\t({game.FinalRankDeltas[i]})\n");
            }
            var bestPayment = 0;
            RoundResult bestResult = null;
            Round bestRound = null;
            var player = -1;
            foreach (var log in game.Rounds)
            {
                foreach (var res in log.Result)
                {
                    var i = 0;
                    foreach (var delta in res.Payments)
                    {
                        if (delta > bestPayment)
                        {
                            bestPayment = delta;
                            bestResult = res;
                            bestRound = log;
                            player = i;
                        }
                        i++;
                    }
                }
            }
            sb.AppendLine($"{String.Format(Resources.leaderboard_bestHand,discordIds[player],game.Names[player],bestRound.RoundNumber,bestResult.HandScore,bestPayment)}{getReactionFromScore(client, bestPayment)}");
            return sb.ToString();
        }

        private static string PrintGameResult(Game game, List<Game> similarGames, Server server)
        {

            if (server.LeaderboardConfigId == null)
            {
                throw new Exception(Resources.commandError_leaderboardNotInitialized);
            }
            var config = ConfigDbService.GetConfig((int)(server.LeaderboardConfigId));
            var partialRankings = Ranking.getPartialRanking(game, config);

            var sb = new StringBuilder();
            if(game.Id > 0)
            {
                sb.AppendLine(String.Format(Resources.leaderboard_submitResult_NameAndId,game.GameName, game.Id));
            }
            sb.AppendLine(String.Format(Resources.leaderboard_submitResult_Date,game.Timestamp.ToString("yyyy/MM/dd")));
            sb.AppendLine(Resources.leaderboard_submitResult_messageHeader);
            foreach (var ranking in partialRankings)
            {
                var nbGames = RankingDbService.GetUserRankingHistory(ranking.UserId, ranking.ServerId).Count() - 1;
                sb.AppendLine($"{ranking.Position}: {getPlayerMention(ranking.UserId)} ({nbGames}):\t{ranking.Score} => {(float)(ranking.ScoreWithBonus)/1000}");
            }

            if(similarGames.Count > 0 )
            {
                sb.AppendLine(String.Format(Resources.leaderboard_submitResult_SimilarGameFound,similarGames.Count()));
                // Recursion? recursion.
                sb.AppendLine(PrintGameResult(similarGames.Last(),new List<Game>(), server));
            }
            return sb.ToString();
        }

        private static string getReactionFromScore(DiscordClient client, int score)
        {
            if(score < 8000)
            {
                return DiscordEmoji.FromName(client, Reactions.WOW);
            }
            return DiscordEmoji.FromName(client, Reactions.WOW);
        }
    }
}
