using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Kandora
{
    public class KandoraCommands
    {
        [Command("hand")]
        public async Task Hand(CommandContext ctx, params string[] handBits)
        {
            var hand = string.Join("", handBits);
            var result = HandParser.GetHandEmojiCode(hand, ctx.Client);
            await ctx.RespondAsync(result);
        }

        [Command("register")]
        public async Task Register(CommandContext ctx, int mahjsoulId)
        {
            var displayName = ctx.Member.Username;
            var discordId = ctx.Member.Id;
            var isOk = UserDb.AddUser(displayName, discordId, mahjsoulId);
            await ctx.RespondAsync(isOk ? 
                $"<@{ctx.Member.Id}> a été enregistré" : 
                $"Erreur, <@{ctx.Member.Id}> n'a pas été enregistré");
        }


        [Command("listusers")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                var users = UserDb.getUsers();
                int i = 1;
                foreach (var user in users)
                {
                    await ctx.RespondAsync($"{i}: <@{user.DiscordId}> {user.MahjsoulId}");
                    i++;
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }
    }
}