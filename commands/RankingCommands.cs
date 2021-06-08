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
            try
            {
                DbService.Begin("scorematch");
                var serverId = ctx.Guild.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(allUsers);
                var server = servers[serverId];
                var serverUsers = server.Users;
                var log = await LogService.Instance.GetLog(gameId, 2);

                var users = await GetUsersFromLog(log, serverUsers, ctx);
                var gameMsg = await ctx.RespondAsync($"I need all players to :o: or :x: please \n"+log.Pretty(ctx.Client,users));
                await context.AddPendingGame(ctx, gameMsg, new PendingGame(users, server, log));

                DbService.Commit("scorematch");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                DbService.Rollback("scorematch");
            }
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

        [Command("getloginfo"), Description("Returns various info about a mahjsoul or tenhou game")]
        public async Task GetLogInfo(CommandContext ctx, [Description("The game id")] string gameId)
        {
            try
            {
                var log = await LogService.Instance.GetLog(gameId, 2);
                await ctx.RespondAsync(log.Pretty(ctx.Client));
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("scorematchid"), Description("Record a game with discord ids"), Aliases("scoreid")]
        public async Task ScoreMatchId(CommandContext ctx, [Description("The players, from winner to last place")] params string[] discordUserList)
        {
            await scoreMatchWithIds(ctx, discordUserList);
        }

        [Command("scorematch"), Description("Record a game"), Aliases("score", "score_match", "s")]
        public async Task ScoreMatch(CommandContext ctx, [Description("The players, from winner to last place")] params DiscordMember[] discordUserList)
        {
            var usersIds = discordUserList.Select(x => x.Id.ToString()).ToArray();
            await scoreMatchWithIds(ctx, usersIds);
        }

        private async Task scoreMatchWithIds(CommandContext ctx, string[] usersIds)
        {
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                if (ctx.Channel == null)
                {
                    throw (new NotInChannelException());
                }
                var channelDiscordId = ctx.Channel.Id.ToString();
                DbService.Begin("scorematch");
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    throw (new ServerNotRegisteredException());
                }
                var activeServer = servers[serverDiscordId];
                var activeUser = users[userDiscordId];
                if (activeServer.TargetChannelId != channelDiscordId)
                {
                    throw (new SilentException());
                }
                if (!users.ContainsKey(userDiscordId))
                {
                    throw (new UserNotRegisteredException(ctx.User.Id.ToString()));
                }

                var (game, rankings) = ScoreDbService.RecordIRLGame(usersIds, activeServer);
                var message = $"{ctx.User.Username} tries to register a game (id= {game.Id}): \n" +
                            $"1rst: <@{usersIds[0]}>\n" +
                            $"2nd: <@{usersIds[1]}>\n" +
                            $"3rd: <@{usersIds[2]}>\n" +
                            $"4th: <@{usersIds[3]}>\n";
                await ctx.Channel.SendMessageAsync(message);

                DbService.Commit("scorematch");
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    await ctx.RespondAsync(e.Message);
                }
                DbService.Rollback("scorematch");
            }
        }

        [Command("ranking"), Description("Ask Kandora your current league ranking"), Aliases("rank", "leaderboard")]
        public async Task MyRanking(CommandContext ctx)
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

                StringBuilder sb = new StringBuilder();
                sb.Append("Leaderboard:\n");
                int i = 1;
                foreach (var rank in rankingList)
                {
                    sb.Append($"{i}: <@{rank.UserId}> ({rank.NewElo}) {(rank.UserId== userDiscordId ? "<<< You are here": "")}\n");
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