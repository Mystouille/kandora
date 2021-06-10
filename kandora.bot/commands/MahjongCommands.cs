using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.mahjong;
using kandora.bot.mahjong.handcalc;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.TilesConverter;
#pragma warning disable CS4014

namespace kandora.bot.commands
{
    public class MahjongCommands: BaseCommandModule
    {
        public object Tiles { get; private set; }
        public object Divider { get; private set; }

        [Command("test"), Description(""), Aliases("t")]
        public async Task Test(
            CommandContext ctx,
            [Description("The hand to display. Circles: [0-9]p, Chars: [0-9]m, Bamboo: [0-9]s, Honnors: [1-7]z, Dragons: [R,W,G]d, Winds: [ESWN]w")] string hand,
            [Description("The options people can vote for, can be empty, \"all\", or be another hand format")] string options = ""
            )
        {
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

                var hand34 = C.one_line_string_to_34_array(basicHand);
                string suitOrder = U.getSuitOrder(basicHand);
                var shantenCalc = new ShantenCalculator();
                int shanten = -2;
                var nbTiles = hand34.Sum();
                if (nbTiles == 13 || nbTiles == 14)
                {
                    shanten = shantenCalc.Calculate_shanten(hand34);
                }
                StringBuilder sb = new();
                sb.AppendLine($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {getShanten(shanten)}");
                var divider = new HandDivider();
                var results = divider.divide_hand(hand34);

                int i = 1;
                foreach (var result in results)
                {
                    var hand136 = C.from_34_indices_to_136_arrays(result);
                    var setsStr = hand136.Select(set => C.to_one_line_string(set));
                    IEnumerable<string> orderedSetStr = new List<string>();
                    foreach (var chr in suitOrder)
                    {
                        orderedSetStr = orderedSetStr.Concat(setsStr.Where(x => x.Contains(chr)));
                    }
                    sb.AppendLine($"{i}:{string.Join(",",orderedSetStr.Select(x=>$"{GetHandMessage(HandParser.GetHandEmojiCodes(x, ctx.Client))}"))}");
                    i++;
                }
                var message = await ctx.Channel.SendMessageAsync(sb.ToString());
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

        private string getShanten(int shanten) {
            if (shanten < -1)
            {
                return "";
            }
            if (shanten == -1)
            {
                return "agari!";
            }
            if (shanten == 0)
            {
                return "tenpai";
            }
            return $"{shanten}-shanten";
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
                int shanten = -2;
                var nbTiles = hand34.Sum();
                if (nbTiles == 13 || nbTiles == 14)
                {
                    shanten = shantenCalc.Calculate_shanten(hand34);
                }
                var message = await ctx.Channel.SendMessageAsync($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {getShanten(shanten)}");
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
            toReturn += (emojis.Count()>12 ? " ":"") + lastEmoji;
            return toReturn;
        }

    }
}