using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace Kandora
{
    public class MahjongCommands
    {
        [Command("hand"), Description("Displays a specified mahjong hand with emojis"), Aliases("h")]
        public async Task Hand(CommandContext ctx, [Description("The hand to display. Circles: [1-9]p, Chars: [1-9]m, Bamboo: [1-9]s, Honnors: [1-7]z, Dragons: [R,W,G]d, Winds: [ESWN]w")] params string[] textHand)
        {
            var hand = string.Join("", textHand);
            var result = HandParser.GetHandEmojiCode(hand, ctx.Client);
            await ctx.RespondAsync(result);
        }

    }
}