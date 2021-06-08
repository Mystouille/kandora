using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.services.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class RankingCommands : BaseCommandModule
    {
        [Command("submitLog"), Description("Submit a mahjsoul or tenhou log to be counted in the league")]
        public async Task SubmitLog(CommandContext ctx, [Description("The game id")] string gameId)
        {
            try
            {
                DbService.Begin("scorematch");
                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverId];
                var log = await LogService.Instance.GetLog(gameId, 2);
                var gameMsg = await ctx.RespondAsync(log.Pretty());
                var oEmoji = DiscordEmoji.FromName(ctx.Client, ":o:");
                var xEmoji = DiscordEmoji.FromName(ctx.Client, ":x:");
                await gameMsg.CreateReactionAsync(oEmoji);
                await gameMsg.CreateReactionAsync(xEmoji);
                //ctx.Client.MessageReactionAdded
                gameMsg.WaitForReactionAsync(ctx.User, xEmoji).ContinueWith(
                    prev =>
                    {
                        ScoreDbService.RecordOnlineGame(log, server);
                        ctx.Channel.SendMessageAsync("Game recorded!");
                    }
                );

                DbService.Commit("scorematch");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                DbService.Rollback("scorematch");
            }
        }

        [Command("getloginfo"), Description("Returns various info about a mahjsoul or tenhou game")]
        public async Task GetLogInfo(CommandContext ctx, [Description("The game id")] string gameId)
        {
            try
            {
                var log = await LogService.Instance.GetLog(gameId, 2);
                await ctx.RespondAsync(log.Pretty());
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
    }
}