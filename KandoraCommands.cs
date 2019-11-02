using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DescriptionAttribute = DSharpPlus.CommandsNext.Attributes.DescriptionAttribute;

namespace Kandora
{
    public class KandoraCommands
    {
        [Command("hand"), Description("Displays a specified mahjong hand with emojis")]
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


        [Command("listusers"), Description("List the users in Kandora league")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                var users = UserDb.GetUsers();
                int i = 1;

                foreach (var user in users)
                {
                    if (ctx.Member == null)
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

        [Command("scorematch"), Description("Record a game"), Aliases("score", "score_match")]
        public async Task ScoreMatch(CommandContext ctx, [Description("The winner of the game")] DiscordMember user1, [Description("The2nd place")] DiscordMember user2, [Description("The 3rd place")] DiscordMember user3, [Description("The last place")] DiscordMember user4 )
        {
            var users = new DiscordMember[4];
            users[0] = user1;
            users[1] = user2;
            users[2] = user3;
            users[3] = user4;
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
                    ScoreDb.RecordGame(users, signed: isAdmin);
                    var game = ScoreDb.GetLastRecordedGame();

                    foreach (var member in users)
                    {
                        if (!isAdmin)
                        {
                            if (member != ctx.User)
                            {
                                await member.SendMessageAsync($"{ctx.User.Username} tries to record a game: {users[0].DisplayName} > {users[1].DisplayName} > {users[2].DisplayName} > {users[3].DisplayName}.\n" +
                                    $"Use the \'!accept {game.Id}\' command if you agree,\n");
                            }
                        }
                        else
                        {
                            await member.SendMessageAsync($"{ctx.User.Username} recorded the following game with the id {game.Id}: {users[0].DisplayName} > {users[1].DisplayName} > {users[2].DisplayName} > {users[3].DisplayName}.");
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

        [Command("accept"), Description("Accept the proposed record of the game")]
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
    }
}