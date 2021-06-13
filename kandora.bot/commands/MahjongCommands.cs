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
                if(options.Length != 2 || options == "all")
                {
                    await computeHand(ctx, hand, options);
                }
                else if (options.Length == 2)
                {
                    await computeHandWithAgari(ctx, hand, options);
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

        private async Task computeHand(CommandContext ctx, string hand, string options)
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
            sb.AppendLine($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {getShanten(shanten)}\n");
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
                sb.AppendLine($"{i}:{string.Join(",", orderedSetStr.Select(x => $"{GetHandMessage(HandParser.GetHandEmojiCodes(x, ctx.Client))}"))}\n");
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

        private async Task computeHandWithAgari(CommandContext ctx,  string hand, string agari)
        {
            var basicHand = HandParser.GetSimpleHand(hand);
            var basicAgari = HandParser.GetSimpleHand(agari);
            var handEmoji = HandParser.GetHandEmojiCodes(hand, ctx.Client);
            string suitOrder = U.getSuitOrder(basicHand);

            var hand136 = C.one_line_string_to_136_array(basicHand);
            var agari136 = C.one_line_string_to_136_array(basicAgari)[0];

            StringBuilder sb = new();
            sb.AppendLine($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}\n");
            var config = new HandConfig();

            // TSUMO

            config.is_tsumo = true;
            var calculator = new HandCalculator();
            var result = calculator.estimate_hand_value(hand136.ToList(), agari136, config: config);
            var handShape136 = C.from_34_indices_to_136_arrays(result.hand);
            var setsStr = handShape136.Select(set => C.to_one_line_string(set));
            IEnumerable<string> orderedSetStr = new List<string>();
            foreach (var chr in suitOrder)
            {
                orderedSetStr = orderedSetStr.Concat(setsStr.Where(x => x.Contains(chr)));
            }
            sb.AppendLine($"Tsumo:{string.Join(",", orderedSetStr.Select(x => $"{GetHandMessage(HandParser.GetHandEmojiCodes(x, ctx.Client))}"))}");

            sb.AppendLine($"{result.han}han {result.fu}fu");
            foreach (var yaku in result.yaku)
            {
                sb.AppendLine($"{yaku}");
            }
            if (result.cost != null)
            {
                foreach (var yaku in result.cost.Keys)
                {
                    sb.AppendLine($"{yaku}: {result.cost[yaku]}");
                }

                foreach (var detail in result.fu_details)
                {
                    sb.AppendLine($"{detail.Item2}: +{detail.Item1}fu");
                }
            }

            //RON

            config.is_tsumo = false;
            calculator = new HandCalculator();
            result = calculator.estimate_hand_value(hand136.ToList(), agari136, config: config);
            handShape136 = C.from_34_indices_to_136_arrays(result.hand);
            setsStr = handShape136.Select(set => C.to_one_line_string(set));
            orderedSetStr = new List<string>();
            foreach (var chr in suitOrder)
            {
                orderedSetStr = orderedSetStr.Concat(setsStr.Where(x => x.Contains(chr)));
            }
            sb.AppendLine();
            sb.AppendLine($"Ron:{string.Join(",", orderedSetStr.Select(x => $"{GetHandMessage(HandParser.GetHandEmojiCodes(x, ctx.Client))}"))}");

            sb.AppendLine($"{result.han}han {result.fu}fu");
            foreach (var yaku in result.yaku)
            {
                sb.AppendLine($"{yaku}");
            }

            if (result.cost != null)
            {
                foreach (var cost in result.cost.Keys)
                {
                    sb.AppendLine($"{cost}: {result.cost[cost]}");
                }
            }

            foreach (var detail in result.fu_details)
            {
                sb.AppendLine($"{detail.Item2}: +{detail.Item1}fu");
            }
            var message = await ctx.Channel.SendMessageAsync(sb.ToString());
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