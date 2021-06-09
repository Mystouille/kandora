using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#pragma warning disable CS4014

namespace kandora.bot.commands
{
    public class MahjongCommands: BaseCommandModule
    {
        private string GetHandMessage(IEnumerable<DiscordEmoji> emojis)
        {
            string toReturn = "";
            var lastEmoji = "";
            foreach(var emoji in emojis)
            {
                toReturn += lastEmoji;
                lastEmoji = emoji;
            }
            toReturn += " " + lastEmoji;
            return toReturn;
        }


        [Command("hand"), Description("Displays a specified mahjong hand with emojis"), Aliases("h")]
        public async Task Hand(
            CommandContext ctx, 
            [Description("The hand to display. Circles: [0-9]p, Chars: [0-9]m, Bamboo: [0-9]s, Honnors: [1-7]z, Dragons: [R,W,G]d, Winds: [ESWN]w")] string hand,
            [Description("The options people can vote for, can be empty, \"all\", or be another hand format")] string options = ""
        ) {
            try
            {
                var handEmoji = HandParser.GetHandEmojiCodes(hand, ctx.Client);
                var optionsEmoji = options == "all" 
                    ? handEmoji
                    : HandParser.GetHandEmojiCodes(options, ctx.Client);
                try
                {
                    await ctx.Message.DeleteAsync();
                }
                catch
                {
                    // do nothing
                }
                var message = await ctx.Channel.SendMessageAsync($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}");
                {
                    foreach (var emoji in optionsEmoji)
                    {
                        await message.CreateReactionAsync(emoji);
                    }
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

    }
}