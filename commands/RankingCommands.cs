using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using kandora.bot.exceptions;
using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.services.http;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class RankingCommands : KandoraCommandModule
    {
        [Command("submitLog"), Description("Submit a mahjsoul or tenhou log to be counted in the league")]
        public async Task SubmitLog(CommandContext ctx, [Description("The game id")] string gameId)
        {
            await executeCommand(
                ctx,
                GetSubmitLogAction(ctx, gameId)
            );
        }

        private Func<Task> GetSubmitLogAction(CommandContext ctx, string gameId)
        {
            return new Func<Task>(async () =>
            {
                var serverId = ctx.Guild.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                var serverUsers = server.Users;
                var log = await LogService.Instance.GetLog(gameId, 2);

                var users = await GetUsersFromLog(log, serverUsers, ctx);
                var gameResult = PrintGameResult(log, ctx.Client, users);
                var gameMsg = await ctx.RespondAsync($"I need all players to :o: or :x: to record or cancel this game\n{gameResult}");
                await context.AddPendingGame(ctx, gameMsg, new PendingGame(users.Select(x=>x.Id.ToString()).ToArray(), server, log));
            });
        }

        //Checks if a log is compatible with a player base
        private async Task<List<User>> GetUsersFromLog(RiichiGame log, List<User> users, CommandContext ctx)
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
                            bool matchName = user.MahjsoulName != null &&  user.MahjsoulName == log.Names[i];
                            bool matchId = user.MahjsoulUserId != null && user.MahjsoulUserId == log.UserIds[i];
                            if (matchId && !matchName)
                            {
                                UserDbService.SetMahjsoulName(user.Id, log.Names[i]);
                                await ctx.RespondAsync($"Detected Bad user name for <@{user.Id}>. Changed from {user.MahjsoulName} to {log.Names[i]}, don't thank me ;)");
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
                            if(user.TenhouName != null && user.TenhouName == log.Names[i])
                            {
                                foundUser = user;
                            }
                            break;
                        default:
                            throw new Exception("Unsupported game type");
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
                    throw new Exception($"The log has names that do not match with any players: {string.Join(", ", notFound)}");
                }
            }
            return foundUsers;
        }

        [Command("getlog"), Description("Returns various info about a mahjsoul or tenhou game")]
        public async Task GetLogInfo(CommandContext ctx, [Description("The game id")] string gameId)
        {
            await executeCommand(
                ctx,
                GetLogInfoAction(ctx, gameId)
            );
        }

        private Func<Task> GetLogInfoAction(CommandContext ctx, string gameId)
        {
            return new Func<Task>(async () =>
            {
                var log = await LogService.Instance.GetLog(gameId, 2);
                var gameResult = PrintGameResult(log, ctx.Client);
                await ctx.RespondAsync(gameResult);
            });
        }

        [Command("submitIrlTxt"), Description("Submit a game with without scores and with discord IDs only")]
        public async Task SubmitIrlTxt(CommandContext ctx, [Description("The players discord ID, from winner to last place")] params string[] discordUserList)
        {
            await scoreIrlMatch(ctx, discordUserList);
        }

        [Command("submitIrlScoreTxt"), Description("Submit a game with with scores and with discord IDs only")]
        public async Task SubmitIrlScoreTxt(CommandContext ctx, [Description("The players discord IDs with their score (ie. 0012354 58000 00178995 2000 00265897 50000 00156698 10000)")] params string[] input)
        {
            var usersScores = new List<(string, string)>();
            for (int i = 0; i < input.Length-1; i = i + 2)
            {
                usersScores.Add((input[i], input[i+1]));
            }
            usersScores.Sort((tuple1, tuple2) => tuple2.Item2.CompareTo(tuple1.Item2));

            var usersIds = usersScores.Select(x => x.Item1).ToArray();
            var scores = usersScores.Select(x => x.Item2).ToArray();
            await scoreIrlMatch(ctx, usersIds, scores);
        }

        [Command("submitIrl"), Description("Submit a game with without scores")]
        public async Task SubmitIrl(CommandContext ctx, [Description("The players (@ mentions), from winner to last place")] params DiscordMember[] discordUserList)
        {
            var usersIds = discordUserList.Select(x => x.Id.ToString()).ToArray();
            await scoreIrlMatch(ctx, usersIds);
        }

        [Command("submitIrlScore4"), Description("Submit a 4 players game with scores")]
        public async Task SubmitIrl4(CommandContext ctx, [Description("The players (@ mentions) with their score (ie. @Nyaa 58000 @Asapin 2000 @Rumi 50000 @Aki 10000)")] 
            DiscordMember list, string score1,
            DiscordMember user2, string score2,
            DiscordMember user3, string score3,
            DiscordMember user4, string score4)
        {
            var usersScores = new (DiscordMember,string)[] { (list, score1), (user2, score2), (user3, score3), (user4, score4) };
            var sortedScores = usersScores.ToList();
            sortedScores.Sort((tuple1, tuple2) => tuple2.Item2.CompareTo(tuple1.Item2));

            var usersIds = sortedScores.Select(x => x.Item1.Id.ToString()).ToArray();
            var scores = sortedScores.Select(x => x.Item2).ToArray();
            await scoreIrlMatch(ctx, usersIds, scores);
        }

        [Command("submitIrlScore3"), Description("Submit a 3 player game with scores")]
        public async Task SubmitIrl3(CommandContext ctx, [Description("The players (@ mentions) with their score (ie. @Nyaa 58000 @Asapin 2000 @Rumi 30000)")]
            DiscordMember list, string score1,
            DiscordMember user2, string score2,
            DiscordMember user3, string score3)
        {
            var usersScores = new (DiscordMember, string)[] { (list, score1), (user2, score2), (user3, score3)};
            var sortedScores = usersScores.ToList();
            sortedScores.Sort((tuple1, tuple2) => tuple2.Item2.CompareTo(tuple1.Item2));

            var usersIds = sortedScores.Select(x => x.Item1.Id.ToString()).ToArray();
            var scores = sortedScores.Select(x => x.Item2).ToArray();
            await scoreIrlMatch(ctx, usersIds, scores);
        }

        private async Task scoreIrlMatch(CommandContext ctx, string[] usersIds, string[] scoresStr = null)
        {
            float[] scores = null;
            if(scoresStr != null)
            {
                scores = scoresStr.Select(x => float.Parse(x)).ToArray();
            }
            await executeCommand(
                ctx,
                GetScoreIrlMatchAction(ctx, usersIds, scores)
            );
        }

        private Func<Task> GetScoreIrlMatchAction(CommandContext ctx, string[] userIds, float[] scores)
        {
            return new Func<Task>(async () =>
            {
                var serverId = ctx.Guild.Id.ToString();
                var userId = ctx.Member.Id.ToString();
                var channelDiscordId = ctx.Channel.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                foreach(var id in userIds)
                {
                    if (server.Users.Where(x => x.Id == id).Count() == 0)
                    {
                        throw new Exception($"User <@{id}> is not register in this server league");
                    }
                }
                var leagueConfig = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                if (leagueConfig.CountPoints && scores == null)
                {
                    throw new Exception("The current league configuration requires scores");
                }
                if (!leagueConfig.AllowSanma && userIds.Length == 3)
                {
                    throw new Exception("The current league configuration does not accept sanma games (cheh)");
                }
                if (userIds.Length < 3 || userIds.Length > 4)
                {
                    throw new Exception($"The current league configuration does not support {userIds.Length} player games (what were you thinking?)");
                }
                var distinctUsers = userIds.Distinct();
                if (distinctUsers.Count() < userIds.Length)
                {
                    var userListStr = string.Join(", ", distinctUsers.Select(x => $"<@{x}>"));
                    throw new Exception($"I only received {userListStr}, a user might be mentioned twice?");
                }

                var gameResult = PrintGameResult(ctx.Client, userIds, scores);
                var gameMsg = await ctx.RespondAsync($"I need all players to :o: or :x: to record or cancel this game\n{gameResult}");
                await context.AddPendingGame(ctx, gameMsg, new PendingGame(userIds, scores, server));
            });
        }

        [Command("getrank"), Description("Ask Kandora your current league ranking"), Aliases("rank")]
        public async Task GetRank(CommandContext ctx)
        {
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                if (ctx.Channel == null)
                {
                    throw (new NotInChannelException());
                }
                DbService.Begin("ranking");
                var users = UserDbService.GetUsers();
                List<Ranking> rankingList = RankingDbService.GetServerRankings(serverDiscordId);
                var latestUserRankings = new Dictionary<string, Ranking>();
                foreach (var rank in rankingList)
                {
                    var userId = rank.UserId;
                    if (!latestUserRankings.ContainsKey(userId))
                    {
                        latestUserRankings.Add(rank.UserId, rank);
                    }
                }

                List<Ranking> sortedRanks = latestUserRankings.Values.ToList();
                sortedRanks.Sort((val1, val2) => val2.NewRank.CompareTo(val1.NewRank));

                StringBuilder sb = new StringBuilder();
                sb.Append("Leaderboard:\n");
                int i = 1;
                foreach (var rank in sortedRanks)
                {
                    sb.Append($"{i}: <@{rank.UserId}> ({rank.NewRank}) {(rank.UserId== userDiscordId ? "<<< You are here": "")}\n");
                    i++;
                }
                await ctx.RespondAsync(sb.ToString());
                DbService.Commit("ranking");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                DbService.Rollback("ranking");
            }
        }

        private static string PrintGameResult(DiscordClient client, string[] userIds, float[] scores = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("IRL Game:");
            for(int i = 0; i < userIds.Length; i++)
            {
                sb.AppendLine($"{i+1}: <@{userIds[i]}>: {scores[i]}");
            }
            return sb.ToString();
        }

        private static string PrintGameResult(RiichiGame game, DiscordClient client, List<User> users = null)
        {
            StringBuilder sb = new StringBuilder();
            if (game.Title != null)
            {
                sb.Append($"Title: {game.Title[0]}\n");
                sb.Append($"Time: {game.Title[1]}\n");
            }
            sb.Append($"Scores: \n");
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
                        throw new Exception($"Couldn't find Discord user with game name: {name}");
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
            sb.Append($"Best hand: {discordIds[player]}({game.Names[player]}) (round {bestRound.RoundNumber}) with {bestResult.HandScore} for {bestPayment} total {DiscordEmoji.FromName(client, Reactions.WOW)} \n");
            return sb.ToString();
        }
    }
}