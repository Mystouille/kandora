using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kandora
{
    public class RankingCommands : BaseCommandModule
    {
        [Command("scorename"), Description("Record a game using users display name (accessible from the listusers command)")]
        public async Task ScoreMatchWithName(CommandContext ctx, [Description("The players display name, from winner to last place")] params string[] nameList)
        {
            if (ctx.Channel == null)
            {
                throw (new NotInChannelException());
            }
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            var channelDiscordId = ctx.Channel.Id.ToString();
            try
            {
                GlobalDb.Begin("scorename");
                var users = UserDb.GetUsers();
                var servers = ServerDb.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    throw (new ServerNotRegisteredException());
                }
                var activeServer = servers[serverDiscordId];
                if (activeServer.TargetChannelId != channelDiscordId)
                {
                    throw (new SilentException());
                }
                if (!users.ContainsKey(userDiscordId))
                {
                    throw (new UserNotRegisteredException(ctx.User.Id.ToString()));
                }
                var activeUser = users[userDiscordId];
                var isAdmin = activeServer.Admins.Contains(activeUser);

                var usersList = new List<User>();
                foreach (var userName in nameList)
                {
                    usersList.Add(users.Values.Where(x => x.DisplayName == userName).First());
                }
                var newGame = ScoreDb.RecordGame(usersList.Select(x => x.Id).ToArray(), sourceMember: userDiscordId, activeServer, signed: isAdmin);
                GlobalDb.Commit("scorename");
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    await ctx.RespondAsync(e.Message);
                }
                GlobalDb.Rollback("scorematch");
            }
        }

        [Command("scorematch"), Description("Record a game"), Aliases("score", "score_match", "s")]
        public async Task ScoreMatch(CommandContext ctx, [Description("The players, from winner to last place")] params DiscordMember[] discordUserList)
        {
            var usersIds = discordUserList.Select(x => x.Id.ToString()).ToArray();
            if (ctx.Channel == null)
            {
                throw (new NotInChannelException());
            }
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            var channelDiscordId = ctx.Channel.Id.ToString();
            try
            {
                GlobalDb.Begin("scorematch");
                var users = UserDb.GetUsers();
                var servers = ServerDb.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    throw (new ServerNotRegisteredException());
                }
                var activeServer = servers[serverDiscordId];
                var activeUser = users[userDiscordId];
                if(activeServer.TargetChannelId != channelDiscordId)
                {
                    throw (new SilentException());
                }
                if (!users.ContainsKey(userDiscordId))
                {
                    throw (new UserNotRegisteredException(ctx.User.Id.ToString()));
                }
                var isAdmin = activeServer.Admins.Contains(activeUser);


                var game = ScoreDb.RecordGame(usersIds, sourceMember: userDiscordId, activeServer, signed: isAdmin);
                var message = $"{ctx.User.Username} tries to register a game (id= {game.Id}): \n" +
                            $"1rst: <@{discordUserList[0].Id}>\n" +
                            $"2nd: <@{discordUserList[1].Id}>\n" +
                            $"3rd: <@{discordUserList[2].Id}>\n" +
                            $"4th: <@{discordUserList[3].Id}>\n";
                message += isAdmin 
                    ? $"Since {ctx.User.Username} is an admin, game has been signed off\n"
                    : $"Use the \'!accept {game.Id}\' command to sign the scoresheet,\n";
                await ctx.Channel.SendMessageAsync(message);
                
                GlobalDb.Commit("scorematch");
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    await ctx.RespondAsync(e.Message);
                }
                GlobalDb.Rollback("scorematch");
            }
        }

        [Command("accept"), Description("Accept the proposed record of the game"), Aliases("a")]
        public async Task Accept(CommandContext ctx, [Description("The id of the game")] int id)
        {
            if (ctx.Channel == null)
            {
                throw (new NotInChannelException());
            }
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                GlobalDb.Begin("accept");
                var server = ServerDb.GetServer(serverDiscordId);
                var game = ScoreDb.GetGame(id, server);
                ScoreDb.SignGameByUser(game, userDiscordId);
                await ctx.RespondAsync($"You accepted the game n°{id}");
                GlobalDb.Commit("accept");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("accept");
            }
        }

        [Command("ranking"), Description("Ask Kandora your current league ranking"), Aliases("rank", "leaderboard")]
        public async Task MyRanking(CommandContext ctx)
        {
            var userDiscordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                GlobalDb.Begin("ranking");
                var users = UserDb.GetUsers();
                List<Ranking> lastUserRkList = new List<Ranking>();
                foreach (var user in users.Values)
                {
                    lastUserRkList.Add(RankingDb.GetUserRankings(user.Id).LastOrDefault());
                }
                lastUserRkList.Sort((a, b) => (-1*a.NewElo.CompareTo(b.NewElo)));

                StringBuilder sb = new StringBuilder();
                sb.Append("Leaderboard:\n");
                int i = 1;
                foreach (var rank in lastUserRkList)
                {
                    sb.Append($"{i}: <@{rank.UserId}> ({rank.NewElo}) {(rank.UserId== userDiscordId ? "<<< You are here": "")}\n");
                    i++;
                }

                if (ctx != null && ctx.Member == null)
                {
                    await ctx.RespondAsync(sb.ToString());
                }
                else
                {
                    await ctx.Member.SendMessageAsync(sb.ToString());
                }
                GlobalDb.Commit("ranking");
            }
            catch (Exception e)
            {
                if (ctx != null && ctx.Member == null)
                {
                    await ctx.RespondAsync(e.Message);
                }
                else
                {
                    await ctx.Member.SendMessageAsync(e.Message);
                }
                GlobalDb.Rollback("ranking");
            }
        }
    }
}