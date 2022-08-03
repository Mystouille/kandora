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

namespace kandora.bot.commands.regular
{
    public class MahjongCommands: KandoraCommandModule
    {
        public object Tiles { get; private set; }
        public object Divider { get; private set; }

        [Command("problem"), Description(""), Aliases("p")]
        public async Task Problem(
            CommandContext ctx,
            string suit=""
        )
        {

        }

        [Command("problem2"), Description(""), Aliases("p2")]
        public async Task Problems(
            CommandContext ctx,
            string suit = ""
        )
        {

        }

        [Command("image"), Description(""), Aliases("i")]
        public async Task Image(
            CommandContext ctx,
            string hand
        )
        {
            try
            {
                List<string> basicHand = HandParser.SimpleTiles(hand).Where(x => x.Length == 2).ToList();
                if (basicHand.Count == 0)
                {
                    throw new Exception("invalid hand");
                }
                var stream = ImageToolbox.GetImageFromTiles(hand);
                var mb = new DiscordMessageBuilder();
                mb.WithFile(stream);
                await mb.SendAsync(ctx.Channel).ContinueWith(precedent => stream.Close());
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }

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

            var hand34 = C.FromStringTo34Count(basicHand);
            string suitOrder = U.GetSuitOrder(basicHand);
            var shantenCalc = new ShantenCalculator();
            int shanten = -2;
            var nbTiles = hand34.Sum();
            if (nbTiles == 13 || nbTiles == 14)
            {
                shanten = shantenCalc.GetNbShanten(hand34);
            }
            StringBuilder sb = new();
            sb.AppendLine($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {getShantenStr(shanten)}\n");
            var divider = new HandDivider();
            var results = divider.DivideHand(hand34);

            int i = 1;
            foreach (var result in results)
            {
                var hand136 = C.From34IdxHandTo136Hand(result);
                var setsStr = hand136.Select(set => C.ToString(set));
                IEnumerable<string> orderedSetStr = new List<string>();
                foreach (var chr in suitOrder)
                {
                    orderedSetStr = orderedSetStr.Concat(setsStr.Where(x => x.Contains(chr)));
                }
                sb.AppendLine($"{i}:{string.Join(",", orderedSetStr.Select(x => $"{GetHandMessage(HandParser.GetHandEmojiCodes(x, ctx.Client))}"))}\n");
                i++;
            }

            var agari34 = new int[34];
            if (nbTiles == 13)
            {
                for(int idx = 0; idx < 34; idx++)
                {
                    hand34[idx]++;
                    if (shantenCalc.GetNbShanten(hand34) < 0 && hand34[idx] <= 4)
                    {
                        agari34[idx]++;
                    }
                    hand34[idx]--;
                }
            }
            var agari136 = C.From34countTo136(agari34.ToList());
            var agariStr = C.ToString(agari136);
            var agariEmoji = HandParser.GetHandEmojiCodes(agariStr, ctx.Client);
            sb.AppendLine($"Agari: {GetHandMessage(agariEmoji)}");

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
            string suitOrder = U.GetSuitOrder(basicHand);

            var hand136 = C.FromStringTo136(basicHand);
            var agari136 = C.FromStringTo136(basicAgari)[0];

            StringBuilder sb = new();
            sb.AppendLine($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}\n");
            var config = new HandConfig();

            // TSUMO

            config.isTsumo = true;
            var calculator = new HandCalculator();
            var result = calculator.EstimateHandValue(hand136.ToList(), agari136, config: config);
            var handShape136 = C.From34IdxHandTo136Hand(result.hand);
            var setsStr = handShape136.Select(set => C.ToString(set));
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

            config.isTsumo = false;
            calculator = new HandCalculator();
            result = calculator.EstimateHandValue(hand136.ToList(), agari136, config: config);
            handShape136 = C.From34IdxHandTo136Hand(result.hand);
            setsStr = handShape136.Select(set => C.ToString(set));
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

        private string getShantenStr(int shanten) {
            if (shanten < -1)
            {
                return "";
            }
            if (shanten == -1)
            {
                return "AGARI";
            }
            if (shanten == 0)
            {
                return "tenpai";
            }
            if (shanten == 1)
            {
                return "ii-shanten";
            }
            if (shanten == 2)
            {
                return "ryan-shanten";
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
                    //await ctx.Message.DeleteAsync();
                }
                catch
                {
                    // do nothing
                }

                var hand34 = TilesConverter.FromStringTo34Count(basicHand);
                var shantenCalc = new ShantenCalculator();
                int shanten = -2;
                var nbTiles = hand34.Sum();
                if (nbTiles == 13 || nbTiles == 14)
                {
                    shanten = shantenCalc.GetNbShanten(hand34);
                }
                var message = await ctx.Channel.SendMessageAsync($"<@!{ctx.User.Id}>: {GetHandMessage(handEmoji)}  {getShantenStr(shanten)}");
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