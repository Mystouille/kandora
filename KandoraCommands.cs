using System;
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
        public async Task Register(CommandContext ctx, string mahjsoulId)
        {

            await ctx.RespondAsync("Tu es enregistré dans la ligue!");
        }


        [Command("list")]
        public async Task List(CommandContext ctx)
        {

            var users = UserDb.getUsers();
            int i = 1;
            await foreach (var user in users)
            {
                await ctx.RespondAsync(""+i+": "+ user.displayName +" "+ user.mahjsoulId);
            }

        }
    }
}