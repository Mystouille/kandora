using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kandora
{
    public class RankingCommands
    {
        [Command("scorename"), Description("Record a game using users display name (accessible from the listusers command)")]
        public async Task ScoreMatchWithName(CommandContext ctx, [Description("The players display name, from winner to last place")] params string[] nameList)
        {
            try
            {
                GlobalDb.Begin("scorename");
                var userList = UserDb.GetUsers();
                bool isAdmin = false;
                foreach (var user in userList)
                {
                    if (user.Id == ctx.User.Id)
                    {
                        if (user.IsAdmin)
                        {
                            isAdmin = true;
                        }
                        break;
                    }
                }
                if (!isAdmin)
                {
                    await ctx.RespondAsync("Cancelled. Only admins can use this command.");
                    return;
                }

                string currentUser = "";
                var users = new ulong[4];
                try
                {
                    int i = 0;
                    foreach (var userName in nameList)
                    {
                        currentUser = userName;
                        users[i] = userList.Find(x => x.DisplayName == userName).Id;
                        i++;
                    }
                }
                catch
                {
                    throw(new Exception($"Cancelled. Couldn't find user named \"{currentUser}\" in the list"));
                }
                var newGame = ScoreDb.RecordGame(users, sourceMember: ctx.User.Id, signed: true);
                GlobalDb.Commit("scorename");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("scorename");
            }
        }

        [Command("scorematch"), Description("Record a game"), Aliases("score", "score_match", "s")]
        public async Task ScoreMatch(CommandContext ctx, [Description("The players, from winner to last place")] params DiscordMember[] discordUserList)
        {
            var usersIds = discordUserList.Select(x => x.Id).ToArray();
            if (ctx.Member == null)
            {
                await ctx.RespondAsync("Cancelled. You must be in a text channel for that command");
                return;
            }
            try
            {
                GlobalDb.Begin("scorematch");
                var userList = UserDb.GetUsers();
                bool isAdmin = false;
                foreach (var user in userList)
                {
                    if (user.Id == ctx.User.Id)
                    {
                        if (user.IsAdmin)
                        {
                            isAdmin = true;
                        }
                        break;
                    }
                }
                var game = ScoreDb.RecordGame(usersIds, sourceMember: ctx.User.Id, signed: isAdmin);

                foreach (var member in discordUserList)
                {

                    if (!isAdmin)
                    {
                        if (member != ctx.User)
                        {
                            var message = $"{ctx.User.Username} tries to register a game (id= {game.Id}): \n" +
                                $"1: <@{discordUserList[0].Id}>\n" +
                                $"2: <@{discordUserList[1].Id}>\n" +
                                $"3: <@{discordUserList[2].Id}>\n" +
                                $"4: <@{discordUserList[3].Id}>\n" +
                                $"Use the \'!accept {game.Id}\' command to sign the scoresheet,\n";
                            await member.SendMessageAsync(message);
                        }
                    }
                    else
                    {
                        var message = $"{ctx.User.Username} registered a game (id= {game.Id}): \n" +
                            $"1: <@{discordUserList[0].Id}>\n" +
                            $"2: <@{discordUserList[1].Id}>\n" +
                            $"3: <@{discordUserList[2].Id}>\n" +
                            $"4: <@{discordUserList[3].Id}>\n" +
                            $"{ctx.User.Username} is an admin, game was auto-signed\n";
                        await member.SendMessageAsync(message);
                    }
                }
                GlobalDb.Commit("scorematch");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("scorematch");
            }
        }

        [Command("accept"), Description("Accept the proposed record of the game"), Aliases("a")]
        public async Task Accept(CommandContext ctx, [Description("The id of the game")] int id)
        {
            try
            {
                GlobalDb.Begin("accept");
                ScoreDb.SignGameByUser(id, ctx.User.Id);
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
            try
            {
                GlobalDb.Begin("ranking");
                var userList = UserDb.GetUsers().Select(u => u.Id);
                List<Ranking> lastUserRkList = new List<Ranking>();
                foreach (var userId in userList)
                {
                    lastUserRkList.Add(RankingDb.GetUserRankings(userId).LastOrDefault());
                }
                lastUserRkList.Sort((a, b) => (-1*a.NewElo.CompareTo(b.NewElo)));

                StringBuilder sb = new StringBuilder();
                sb.Append("Leaderboard:\n");
                int i = 1;
                foreach (var rank in lastUserRkList)
                {
                    sb.Append($"{i}: <@{rank.UserId}> ({rank.NewElo}) {(rank.UserId==ctx.User.Id ? "<<< You are here": "")}\n");
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
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("ranking");
            }
        }
    }
}