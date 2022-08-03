using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.resources;
using kandora.bot.services.discord.problems;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public sealed class KandoraSlashContext
    {
        private static readonly KandoraSlashContext instance = new KandoraSlashContext();


        static KandoraSlashContext()
        {
        }

        private KandoraSlashContext()
        {
            PendingGames = new Dictionary<ulong, PendingGame>();
            OngoingQuizz = new Dictionary<ulong, OngoingQuizz>();
            GuildsWithOngoingQuizz = new HashSet<ulong>();
        }
        public static KandoraSlashContext Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<ulong, PendingGame> PendingGames { get; }
        public Dictionary<ulong, OngoingQuizz> OngoingQuizz { get; }
        public ISet<ulong> GuildsWithOngoingQuizz { get; }

        public async Task NotifyReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if (PendingGames.ContainsKey(msg.Id))
            {
                await PendingGame.OnPendingGameReaction(sender, msg, emoji, user, added);
            }
            else if (OngoingQuizz.ContainsKey(msg.Id))
            {
                await OngoingQuizz[msg.Id].OnProblemReaction(sender, msg, emoji, user, added).ConfigureAwait(true);
            }
        }

        private async void OnQuestionEnd(DiscordMessage msg)
        {
            var nextProblem = OngoingQuizz[msg.Id].GetNextProblem();
            OngoingQuizz.Remove(msg.Id);
            if (nextProblem == null)
            {
                GuildsWithOngoingQuizz.Remove(msg.Channel.Guild.Id);
            }
            else
            {
                await AddOngoingQuizz(nextProblem, residentChannel: msg.Channel);
            }
        }

        public async Task AddPendingGame(InteractionContext ctx, DiscordMessage msg, PendingGame game)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(DiscordEmoji.FromName(ctx.Client, Reactions.OK))
            ).ContinueWith(x => {
                ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(DiscordEmoji.FromName(ctx.Client, Reactions.NO)));
            }).ContinueWith(x => {
                PendingGames.Add(msg.Id, game);
            });
        }

        public async Task StartProblemSeries(InteractionContext ctx, IQuizzGenerator generator, int nbProblems, int timeout, string startMsgContent, string threadNameRes)
        {
            if (GuildsWithOngoingQuizz.Contains(ctx.Guild.Id))
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = Resources.commandAttributeError_title,
                    Description = Resources.quizz_error_problemAlreadyExists,
                    Color = new DiscordColor(0xFF0000) // red
                };
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(true).AddEmbed(embed)).ConfigureAwait(true);
            }
            else
            {

                var startingMessageContent = string.Format(startMsgContent, nbProblems) + (timeout>0 ? string.Format(Resources.quizz_timer_disclaimer, timeout) : "");
                var rb = new DiscordInteractionResponseBuilder().WithContent(startingMessageContent);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
                var startingMessage = await ctx.Interaction.GetOriginalResponseAsync().ConfigureAwait(true);
                var culture = new CultureInfo(Resources.cultureInfoStr, true);
                var dateStr = DateTime.Now.ToString("ddd dd MMM HH'h'mm", culture);
                var threadNameStr = string.Format(threadNameRes, dateStr, nbProblems);
                var thread = await ctx.Channel.CreateThreadAsync(startingMessage, string.Format(threadNameStr, dateStr, nbProblems), AutoArchiveDuration.Hour).ConfigureAwait(true);
                var problem = new OngoingQuizz(
                    generator: generator,
                    nbQuestions: nbProblems,
                    timeout: timeout,
                    onQuestionEnd: this.OnQuestionEnd
                );
                await AddOngoingQuizz(problem, residentChannel: thread);
            }
        }

        private async Task AddOngoingQuizz(OngoingQuizz quizz, DiscordChannel residentChannel)
        {
            DiscordMessage msg = null;
            var messageHeader = quizz.HeaderMessage;
            var messageWait = Resources.quizz_generatingProblem;
            var startRoundMessageContent = $"{messageHeader}\n{messageWait}";
            msg = await residentChannel.SendMessageAsync(startRoundMessageContent).ConfigureAwait(true);

            GuildsWithOngoingQuizz.Add(msg.Channel.Guild.Id);
            OngoingQuizz.Add(msg.Id, quizz);
            foreach (var emoji in quizz.QuestionDataData.OptionEmojis)
            {
                await msg.CreateReactionAsync(emoji).ConfigureAwait(true);
            }

            var message = $"{messageHeader}\n{quizz.GetCurrentWinners()}";

            var msgb = new DiscordMessageBuilder().WithContent(message).WithFile(quizz.QuestionDataData.Image);
            msg = await msg.ModifyAsync(msgb).ConfigureAwait(true);
            quizz.QuestionDataData.Image.Dispose();
            quizz.ResetTimer();

            if (quizz.Timeout > 0)
            {
                await Task.Delay(quizz.Timeout*1000).ContinueWith(parent => {
                    quizz.OnQuestionTimeout(msg);
                }).ConfigureAwait(false);
            }
        }
    }
}
