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
            OngoingProblems = new Dictionary<ulong, OngoingProblem>();
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
        public Dictionary<ulong, OngoingProblem> OngoingProblems { get; }
        public ISet<ulong> GuildsWithOngoingQuizz { get; }

        public async Task NotifyReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if (PendingGames.ContainsKey(msg.Id))
            {
                await PendingGame.OnPendingGameReaction(sender, msg, emoji, user, added);
            }
            else if (OngoingProblems.ContainsKey(msg.Id))
            {
                await OngoingProblems[msg.Id].OnProblemReaction(sender, msg, emoji, user, added).ConfigureAwait(true);
            }
        }

        private async void OnQuestionEnd(DiscordMessage msg)
        {
            if (!OngoingProblems.ContainsKey(msg.Id)) { 
                return;
            }
            var nextProblem = OngoingProblems[msg.Id].GetNextProblem();
            OngoingProblems.Remove(msg.Id);
            if (nextProblem == null)
            {
                GuildsWithOngoingQuizz.Remove(msg.Channel.Guild.Id);
            }
            else
            {
                await AddOngoingQuizz(nextProblem, residentChannel: msg.Channel);
            }
        }

        public async Task AddPendingGame(InteractionContext ctx, string gameInfo, PendingGame game)
        {
            var rb = new DiscordInteractionResponseBuilder().WithContent(gameInfo);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            var response = await ctx.GetOriginalResponseAsync();
            await response.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.OK));
            await response.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.NO));
            PendingGames.Add(response.Id, game);
        }

        public async Task StartProblemSeries(InteractionContext ctx, OngoingProblem problem, string startMsgContent, string threadNameRes)
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
                var nbProblems = problem.NbTotalQuestions;
                var timeout = problem.Timeout;
                var startingMessageContent = string.Format(startMsgContent, nbProblems) + (timeout > 0 ? string.Format(Resources.quizz_timer_disclaimer, timeout) : "");
                var rb = new DiscordInteractionResponseBuilder().WithContent(startingMessageContent);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
                var startingMessage = await ctx.Interaction.GetOriginalResponseAsync().ConfigureAwait(true);
                var culture = new CultureInfo(Resources.cultureInfoStr, true);
                var dateStr = DateTime.Now.ToString("ddd dd MMM HH'h'mm", culture);
                var threadNameStr = string.Format(threadNameRes, dateStr, nbProblems);
                var thread = await ctx.Channel.CreateThreadAsync(startingMessage, string.Format(threadNameStr, dateStr, nbProblems), AutoArchiveDuration.Hour).ConfigureAwait(true);

                await AddOngoingQuizz(problem, residentChannel: thread).ConfigureAwait(true);
            }
        }

        private async Task AddOngoingQuizz(OngoingProblem problem, DiscordChannel residentChannel)
        {
            var messageHeader = problem.HeaderMessage;
            var messageWait = Resources.quizz_generatingProblem;
            var startRoundMessageContent = $"{messageHeader}\n{messageWait}";
            var msg = await residentChannel.SendMessageAsync(startRoundMessageContent).ConfigureAwait(true);

            GuildsWithOngoingQuizz.Add(msg.Channel.Guild.Id);
            OngoingProblems.Add(msg.Id, problem);
            problem.OnQuestionEnd = this.OnQuestionEnd;

            foreach (var emoji in problem.QuestionData.OptionEmojis)
            {
                await msg.CreateReactionAsync(emoji).ConfigureAwait(true);
            }

            var msgb = new DiscordMessageBuilder().WithContent(messageHeader).AddFile(problem.QuestionData.Image);
            msg = await msg.ModifyAsync(msgb).ConfigureAwait(true);
            problem.QuestionData.Image.Dispose();
            problem.ResetTimer();

            if (problem.Timeout > 0)
            {
                await Task.Delay(problem.Timeout*1000).ContinueWith(parent => {
                    problem.OnQuestionTimeout(msg);
                }).ConfigureAwait(false);
            }
        }
    }
}
