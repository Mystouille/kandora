using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.mahjong;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable CS4014

namespace kandora.bot.commands
{
    public class MahjongCommands: BaseCommandModule
    {
        public object Tiles { get; private set; }

        [Command("test"), Description("calls Python and do things"), Aliases()]
        public async Task Test(
            CommandContext ctx
        )
        {
            try
            {
                var list = new List<int>(34);
                list.Add(0);
                list.Add(1);
                list.Add(2);
                list.Add(3);
                list.Add(4);
                while (list.Count != 34)
                {
                    list.Add(0);
                }
                var hand = TilesConverter.one_line_string_to_34_array("66m233345p223s77z", false);
                var shanten = new ShantenCalculator();
                var result = shanten.Calculate_shanten(hand);
                await ctx.RespondAsync(string.Join(",", result));
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

            [Command("hand"), Description("Displays a specified mahjong hand with emojis"), Aliases("h")]
        public async Task Hand(
            CommandContext ctx, 
            [Description("The hand to display. Circles: [0-9]p, Chars: [0-9]m, Bamboo: [0-9]s, Honnors: [1-7]z, Dragons: [R,W,G]d, Winds: [ESWN]w")] string hand,
            [Description("The options people can vote for, can be empty, \"all\", or be another hand format")] string options = ""
        ) {
            try
            {
                var basicHand = HandParser.GetSimpleHand(hand);
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

                var hand34 = TilesConverter.one_line_string_to_34_array(basicHand);
                var shantenCalc = new ShantenCalculator();
                string shanten = null;
                var nbTiles = hand34.Sum();
                if (nbTiles == 13 || nbTiles == 14)
                {
                    shanten = shantenCalc.Calculate_shanten(hand34).ToString();
                }
                var message = await ctx.Channel.SendMessageAsync($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {(shanten == null ? "" : $"{shanten}-shanten")}");
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
        private string GetHandMessage(IEnumerable<DiscordEmoji> emojis)
        {
            string toReturn = "";
            var lastEmoji = "";
            foreach (var emoji in emojis)
            {
                toReturn += lastEmoji;
                lastEmoji = emoji;
            }
            toReturn += " " + lastEmoji;
            return toReturn;
        }

    }
}