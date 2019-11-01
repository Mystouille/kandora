using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Kandora
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("hand")]
        [Summary("Return the emoji equivalent of the mahjong hand")]
        public async Task SquareAsync(
        [Summary("The hand the read. Pins: [1-9]p, Sou: [1-9]s, Man: [1-9]m, Honnors: [1-7]z or [E,S,W,N]w or [R,W,G]d. Example: 123m234p777sRRRdEEw")]
        string hand)
        {
            var parser = new HandParser(Context.Client);
            var result = parser.getHandEmojiCode(hand);
            await ReplyAsync(result);
        }
    }
}
