using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using kandora.bot.resources;
using System;
using System.Collections.Generic;
using kandora.bot.utils;
using System.Linq;
using kandora.bot.mahjong;
using System.Text;
using System.IO;
using kandora.bot.services.nanikiru;
using System.Threading;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("mjg", Resources.mahjong_groupDescription)]
    class MahjongSlashCommands : KandoraSlashCommandModule
    {
        //[SlashCommand("img", Resources.mahjong_image_description)]
        public async Task Image(InteractionContext ctx,
            [Option(Resources.mahjong_option_handstr, Resources.mahjong_option_handstr_description)] string handStr)
        {
            FileStream stream = null;
            try
            {
                var basicHand = HandParser.SplitTiles(handStr);
                if (basicHand.Count == 0)
                {
                    throw new Exception("invalid hand");
                }

                stream = ImageToolbox.GetImageFromTiles(handStr);
                var rb = new DiscordInteractionResponseBuilder().AddFile(stream);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ContinueWith(precedent => stream.Dispose()).ConfigureAwait(true);

            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
            finally
            {
                if(stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        //[SlashCommand("info", Resources.mahjong_info_description)]
        public async Task Info(InteractionContext ctx,
            [Option(Resources.mahjong_option_handstr, Resources.mahjong_option_handstr_description)] string handStr)
        {
            FileStream stream = null;
            try
            {

                var basicHand = HandParser.GetSimpleHand(handStr)[0];
                var hand34 = TilesConverter.FromStringTo34Count(basicHand);

                var shantenCalc = new ShantenCalculator();
                var sb = new StringBuilder();
                sb.Append(shantenCalc.GetNbShantenStr(hand34));
                stream = ImageToolbox.GetImageFromTiles(handStr);
                var rb = new DiscordInteractionResponseBuilder().WithContent(sb.ToString()).AddFile(stream);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ContinueWith(precedent => stream.Dispose()).ConfigureAwait(true);

            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        [SlashCommand("nanikiru", Resources.mahjong_nanikiru_description)]
        public async Task Nanikiru(InteractionContext ctx,
            [Option(Resources.mahjong_option_handstr, Resources.mahjong_option_handstr_description)] string handStr,
            [Option(Resources.mahjong_option_potentialDiscards, Resources.mahjong_option_potentialDiscards_description)] string discards = "",
            [Option(Resources.mahjong_option_doras, Resources.mahjong_option_doras_description)] string doras = "",
            [Choice(Resources.mahjong_option_seat_east,Resources.mahjong_option_seat_east)]
            [Choice(Resources.mahjong_option_seat_south,Resources.mahjong_option_seat_south)]
            [Choice(Resources.mahjong_option_seat_west,Resources.mahjong_option_seat_west)]
            [Choice(Resources.mahjong_option_seat_north,Resources.mahjong_option_seat_north)]
            [Option(Resources.mahjong_option_seat, Resources.mahjong_option_seat_description)] string seat = "",
            [Option(Resources.mahjong_option_round, Resources.mahjong_option_round_description)] string round = "",
            [Option(Resources.mahjong_option_turn, Resources.mahjong_option_turn_description)] string turn = "",
            [Option(Resources.mahjong_option_thread, Resources.mahjong_option_thread_description)] bool createThread = false,
            [Choice(Resources.mahjong_option_ukeire_No,"No")]
            [Choice(Resources.mahjong_option_ukeire_Yes,"Yes")]
            [Choice(Resources.mahjong_option_ukeire_Full,"Full")]
            [Option(Resources.mahjong_option_ukeire, Resources.mahjong_option_ukeire_description)] string ukeireDisplay = "No"
            )
        {
            if (handStr == "x")
            {
                GetStoredProblem(ctx);
                return;
            }
            try
            {
                var basicHand = HandParser.GetSimpleHand(handStr);
                var closedHand = basicHand[0];
                var melds = basicHand[1];
                var dorasEmoji = HandParser.GetHandEmojiCodes(doras, ctx.Client);
                var handEmoji = HandParser.GetHandEmojiCodes(closedHand, ctx.Client);
                var optionsEmoji = handEmoji;

                var hand34 = TilesConverter.FromStringTo34Count(closedHand);
                var shantenCalc = new ShantenCalculator();
                var nbTiles = hand34.Sum();
                var sb = new StringBuilder();
                if (discards != "")
                {
                    optionsEmoji = HandParser.GetHandEmojiCodes(discards, ctx.Client).Distinct();
                }
                var context = getHandContext(seat, round, turn, dorasEmoji);
                sb.AppendLine(Resources.mahjong_nanikiru_wwyd);
                sb.AppendLine(context);


                var stream = ImageToolbox.GetImageFromTiles(closedHand, melds, separateLastTile: true);
                var rb = new DiscordInteractionResponseBuilder().WithContent(sb.ToString());
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb.AddFile(stream)).ContinueWith(precedent => stream.Dispose()).ConfigureAwait(true);
                var msg = await ctx.Interaction.GetOriginalResponseAsync().ConfigureAwait(true);

                DiscordThreadChannel thread = null;
                if (createThread)
                {
                    thread = await ctx.Channel.CreateThreadAsync(msg, $"nanikiru {handStr}", AutoArchiveDuration.Day).ConfigureAwait(true);
                    msg = await thread.SendMessageAsync(Resources.mahjong_nanikiru_discussHere).ConfigureAwait(true);
                }
                foreach (var emoji in optionsEmoji)
                {
                    await msg.CreateReactionAsync(emoji).ConfigureAwait(true);
                }

                sb.Clear();
                if (ukeireDisplay != "No")
                {
                    var ukeires = shantenCalc.getUkeire(handStr, doras);
                    
                    var shanten = ukeires.Item1;
                    var discardList = ukeires.Item2;
                    sb.AppendLine($"({shantenCalc.GetShantenStr(ukeires.Item1)})");
                    foreach (var item in discardList)
                    {
                        var discard = item.Item1;
                        var discardStr = TilesConverter.From34IdxTileToString(discard);
                        var discardEmoji = HandParser.GetHandEmojiString(discardStr, ctx.Client);
                        var nbUkeire = item.Item2.Item1;
                        var ukeireListStr = TilesConverter.From34CountHandToString(item.Item2.Item2.ToList());
                        var ukeireListEmoji = HandParser.GetHandEmojiString(ukeireListStr, ctx.Client);
                        if(ukeireDisplay == "Full")
                        {
                            sb.AppendLine($"{discardEmoji}:\t{nbUkeire} ({ukeireListEmoji})");
                        }
                        else
                        {
                            sb.Append($" {discardEmoji}x{nbUkeire}");
                        }
                    }

                    if (createThread && thread != null)
                    {
                        await thread.SendMessageAsync(sb.ToString()).ConfigureAwait(true);
                    }
                    else
                    {
                        await msg.RespondAsync(sb.ToString()).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string getHandContext(string seat, string round, string turn, IEnumerable<DiscordEmoji> doras)
        {
            var context = new List<string>();
            if (seat != "")
            {
                context.Add(string.Format(Resources.mahjong_nanikiru_seat, seat));
            }
            if (round != "")
            {
                context.Add(string.Format(Resources.mahjong_nanikiru_round, this.RoundToString(round)));
            }
            if (turn != "")
            {
                context.Add(string.Format(Resources.mahjong_nanikiru_turn, turn));
            }
            if (doras.Count()>0)
            {
                context.Add(string.Format(Resources.mahjong_nanikiru_dora, string.Join(',', doras)));
            }
            if (seat != "" || round != "" || turn != "" || doras.Count() > 0)
            {
                return string.Join(" | ", context);
            }
            return string.Empty;
        }

        private async void GetStoredProblem(InteractionContext ctx)
        {
            try
            {
                var rb = new DiscordInteractionResponseBuilder().WithContent("bla");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
                
                var problem = StoredNanikiru.Instance.NextProblem();

                var handEmoji = HandParser.GetHandEmojiCodes(problem.Hand, ctx.Client);
                var doras = HandParser.GetHandEmojiCodes(problem.Dora, ctx.Client);
                var sb = new StringBuilder();
                sb.AppendLine(getHandContext(problem.Seat,problem.Round,problem.Turn, doras));
                var answer = string.Join(',', HandParser.GetHandEmojiCodes(problem.Answer, ctx.Client));
                var basicHand = HandParser.GetSimpleHand(problem.Hand);
                var closedHand = basicHand[0];
                var melds = basicHand[1];

                var stream = ImageToolbox.GetImageFromTiles(closedHand, melds, separateLastTile: true);

                sb.AppendLine($"Answer: ||{answer}||");
                if(Resources.cultureInfoStr == "fr-FR" && problem.ExplanationFr.Length > 0)
                {
                    sb.AppendLine($"Explanation: ||{problem.ExplanationFr}||");
                }else
                {
                    sb.AppendLine($"ExplanationEng: ||{problem.ExplanationEng}||");
                }
                rb = new DiscordInteractionResponseBuilder().WithContent(sb.ToString()).AddFile(stream).AsEphemeral();
                var wb = new DiscordWebhookBuilder().WithContent(sb.ToString()).AddFile(stream);
                await ctx.EditResponseAsync(wb).ContinueWith(precedent => stream.Dispose()).ConfigureAwait(true);

            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string RoundToString(string round)
        {
            var roundUp = round.ToUpper();
            string roundString;
            switch (roundUp[0])
            {
                case 'E':
                    roundString = Resources.mahjong_option_seat_east; break;
                case 'S':
                    roundString = Resources.mahjong_option_seat_south; break;
                case 'W':
                    roundString = Resources.mahjong_option_seat_west; break;
                case 'N':
                    roundString = Resources.mahjong_option_seat_north; break;
                default: throw new Exception($"Bad round format: {round}");
            }
            return roundString+round[1..];
        }

    }
}
