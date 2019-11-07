using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DescriptionAttribute = DSharpPlus.CommandsNext.Attributes.DescriptionAttribute;

namespace Kandora
{
    public class KandoraCommands
    {
        [Command("hand"), Description("Displays a specified mahjong hand with emojis"), Aliases("h")]
        public async Task Hand(CommandContext ctx, [Description("The hand to display. Circles: [1-9]p, Chars: [1-9]m, Bamboo: [1-9]s, Honnors: [1-7]z, Dragons: [R,W,G]d, Winds: [ESWN]w")] params string[] textHand)
        {
            var hand = string.Join("", textHand);
            var result = HandParser.GetHandEmojiCode(hand, ctx.Client);
            await ctx.RespondAsync(result);
        }

        [Command("register"), Description("Register yourself in the kandora riichi league")]
        public async Task Register(CommandContext ctx, [Description("Your mahjsoul ID")] int mahjsoulId)
        {
            var displayName = ctx.User.Username;
            var discordId = ctx.User.Id;
            try
            {
                var isOk = UserDb.AddUser(displayName, discordId, mahjsoulId);
                await ctx.RespondAsync(isOk ?
                    $"<@{ctx.User.Id}> has been registered" :
                    $"Cancelled. <@{ctx.User.Id}> couldn't be registered");
            }
            catch(Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }


        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                var users = UserDb.GetUsers();
                int i = 1;

                foreach (var user in users)
                {
                    if (ctx != null && ctx.Member == null)
                    {
                        await ctx.RespondAsync($"{i}: <@{user.Id}> {user.MahjsoulId}");
                    }
                    else
                    {
                        await ctx.Member.SendMessageAsync($"{i}: <@{user.Id}> {user.MahjsoulId}");
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("scorename"), Description("Record a game from users name")]
        public async Task ScoreMatchWithName(CommandContext ctx, [Description("The winner of the game")] string id1, [Description("The2nd place")] string id2, [Description("The 3rd place")] string id3, [Description("The last place")] string id4)
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

                var users = new ulong[4];
                string currentUser = "";
                try
                {
                    currentUser = id1;
                    users[0] = userList.FindLast(x => x.DisplayName == id1).Id;
                    currentUser = id2;
                    users[1] = userList.FindLast(x => x.DisplayName == id2).Id;
                    currentUser = id3;
                    users[2] = userList.FindLast(x => x.DisplayName == id3).Id;
                    currentUser = id4;
                    users[3] = userList.FindLast(x => x.DisplayName == id4).Id;
                }
                catch
                {
                    await ctx.RespondAsync($"Cancelled. Couldn't find user named \"{currentUser}\" in the list");
                    return;
                }
                var newGame = ScoreDb.RecordGame(users, sourceMember: ctx.User.Id, signed: true);
                if (!newGame.Item2)
                {
                    await ctx.RespondAsync($"Error, game n°{newGame.Item1.Id} not recorded. Cleaning up potential dirty DB entries...");
                    var nbLines = ScoreDb.RevertGame(newGame.Item1.Id);
                    if (nbLines > 0)
                    {
                        await ctx.RespondAsync($"DB cleaned up.");
                    }
                    else
                    {
                        await ctx.RespondAsync($"Error cleaning up DB. Contact an admin");
                    }
                }
                else
                {
                    await ctx.RespondAsync($"Game n°{newGame.Item1.Id} recorded.");
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("scorematch"), Description("Record a game"), Aliases("score", "score_match", "s")]
        public async Task ScoreMatch(CommandContext ctx, [Description("The winner of the game")] DiscordMember user1, [Description("The2nd place")] DiscordMember user2, [Description("The 3rd place")] DiscordMember user3, [Description("The last place")] DiscordMember user4)
        {
            var usersIds = new ulong[4];
            usersIds[0] = user1.Id;
            usersIds[1] = user2.Id;
            usersIds[2] = user3.Id;
            usersIds[3] = user4.Id;
            var members = new DiscordMember[4];
            members[0] = user1;
            members[1] = user2;
            members[2] = user3;
            members[3] = user4;
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

                try
                {
                    var gameId = ScoreDb.RecordGame(usersIds, sourceMember: ctx.User.Id, signed: isAdmin);

                    foreach (var member in members)
                    {
                        var message = $"{ctx.User.Username} tries to record a game: {members[0].DisplayName} > {members[1].DisplayName} > {members[2].DisplayName} > {members[3].DisplayName}.\n" +
                                    $"Use the \'!accept {gameId}\' command if you agree,\n";
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
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync(e.Message);
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        [Command("accept"), Description("Accept the proposed record of the game"), Aliases("a")]
        public async Task Accept(CommandContext ctx, [Description("The id of the game")] int id)
        {
            try
            {
                ScoreDb.SignGameByUser(id, ctx.User.Id);
                await ctx.RespondAsync($"You accepted the game n°{id}");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
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
            }
        }
    }
}