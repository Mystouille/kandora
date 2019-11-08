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
                GlobalDb.Commit();
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
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
                var gameId = ScoreDb.RecordGame(usersIds, sourceMember: ctx.User.Id, signed: isAdmin);

                foreach (var member in discordUserList)
                {
                    var message = $"{ctx.User.Username} tries to record a game: \n" +
                        $"1: <@{discordUserList[0].Id}>\n" +
                        $"2: <@{discordUserList[1].Id}>\n" +
                        $"3: <@{discordUserList[2].Id}>\n" +
                        $"4: <@{discordUserList[3].Id}>\n" +
                        $"Use the \'!accept {gameId}\' command to sign the scoresheet,\n";
                    if (!isAdmin)
                    {
                        if (member != ctx.User)
                        {
                            await member.SendMessageAsync(message);
                        }
                    }
                    else
                    {
                        await member.SendMessageAsync(message);
                    }
                }
                GlobalDb.Commit();
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
            }
        }

        [Command("accept"), Description("Accept the proposed record of the game"), Aliases("a")]
        public async Task Accept(CommandContext ctx, [Description("The id of the game")] int id)
        {
            try
            {
                ScoreDb.SignGameByUser(id, ctx.User.Id);
                await ctx.RespondAsync($"You accepted the game n°{id}");
                GlobalDb.Commit();
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
            }
        }

        [Command("ranking"), Description("Ask Kandora your current league ranking"), Aliases("rank", "leaderboard")]
        public async Task MyRanking(CommandContext ctx)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
            }
        }
    }
}