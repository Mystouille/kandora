using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.resources;
using kandora.bot.services.discord.problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class OngoingQuizz : OngoingProblem
    {
        public OngoingQuizz(IQuizzGenerator generator, int timeout, int nbQuestions = 1)
        {
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.LosersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = 1;
            this.NbTotalQuestions = nbQuestions;
            this.Generator = generator;
            this.QuestionData = generator.GetNewQuestion();
            this.ScoreTable = new int[] { 6, 4, 2, 1, 1 };
        }

        private OngoingQuizz(IQuizzGenerator generator, int timeout, Action<DiscordMessage> onQuestionEnd, int quizzProgress, int nbQuestions, Dictionary<ulong, int> playersAndPoints)
        {
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.LosersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = playersAndPoints;
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = quizzProgress;
            this.NbTotalQuestions = nbQuestions;
            this.Generator = generator;
            this.QuestionData = generator.GetNewQuestion();
            this.OnQuestionEnd = onQuestionEnd;
            this.ScoreTable = new int[] { 6, 4, 2, 1, 1 };
        }

        public IQuizzGenerator Generator { get; set; }
        private readonly Dictionary<ulong, ISet<ulong>> usersAnswers;
        public ISet<ulong> Answer { get => QuestionData.AnswerEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public ISet<ulong> Options { get => QuestionData.OptionEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public int QuizzProgress { get; }
        public FileStream Image { get; }

        public void ResetTimer()
        {
            StartTime = DateTime.Now;
        }
        public bool ChangeUserAnswer(ulong userId, ulong answer, bool isAdd)
        {
            if (isAdd)
            {
                if (!usersAnswers.ContainsKey(userId))
                {
                    usersAnswers.Add(userId, new HashSet<ulong>());
                }
                usersAnswers[userId].Add(answer);
            }
            else
            {
                if (usersAnswers.ContainsKey(userId))
                {
                    usersAnswers[userId].Remove(answer);
                }
            }

            if (!usersAnswers.ContainsKey(userId))
            {
                return false;
            }

            var isWinner = usersAnswers[userId].Intersect(Answer).Count() == Answer.Count && usersAnswers[userId].Count == Answer.Count;

            if (isWinner)
            {
                var endTime = DateTime.Now;
                var duration = endTime - StartTime;
                var millisecondDuration = (int)duration.TotalMilliseconds;
                if (WinnersAndTiming.ContainsKey(userId))
                {
                    WinnersAndTiming[userId] = millisecondDuration;
                }
                else
                {
                    WinnersAndTiming.Add(userId, millisecondDuration);
                }
            }
            else
            {
                WinnersAndTiming.Remove(userId);
            }
            return isWinner;
        }

        public override async Task OnProblemReaction(DiscordClient client, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if(client.CurrentUser.Id == user.Id)
            {
                return;
            }
            if (!Options.Contains(emoji.Id))
            {
                return;
            }
            bool isWinner = ChangeUserAnswer(user.Id, emoji.Id, added);

            if (Timeout>0)
            {
                var sb = new StringBuilder();
                var message = $"{HeaderMessage}\n{Resources.quizz_results}\n{GetCurrentWinners()}";
                sb.AppendLine(message);
                var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
                await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
                return;
            }
            else if (isWinner)
            {
                UpdateScores();
                var sb = new StringBuilder();
                sb.AppendLine(GetProgress());
                sb.AppendLine(String.Format(Resources.quizz_suddenDeathWinnerMessage,user.Mention));
                sb.AppendLine(String.Format(Resources.quizz_answer, string.Join("", QuestionData.AnswerEmojis)));
                sb.AppendLine(QuizzProgress == NbTotalQuestions ? Resources.quizz_FinalRanking : Resources.quizz_tempRanking);
                if (NbTotalQuestions > 1)
                {
                    sb.AppendLine(GetFinalScore());
                }
                var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
                await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
                await msg.DeleteAllReactionsAsync().ConfigureAwait(true);
                OnQuestionEnd.Invoke(msg);
            }
        }

        public override async void OnQuestionTimeout(DiscordMessage msg)
        {
            UpdateScores();
            var sb = new StringBuilder();
            sb.AppendLine(GetProgress());
            sb.AppendLine(String.Format(Resources.quizz_answer, string.Join("", QuestionData.AnswerEmojis)));

            if (WinnersAndTiming.Count == 0)
            {
                sb.AppendLine(Resources.quizz_timeoutNoWinnerMessage);
            }

            sb.AppendLine(QuizzProgress == NbTotalQuestions ? Resources.quizz_FinalRanking : Resources.quizz_tempRanking);
            sb.AppendLine(GetFinalScore());
            
            var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
            await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
            await msg.DeleteAllReactionsAsync().ConfigureAwait(true);
            OnQuestionEnd.Invoke(msg);
        }

        private string GetFinalScore()
        {
            var sb = new StringBuilder();
            var pointList = PlayersAndPoints.ToList();
            pointList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            pointList.Reverse();
            for (int i = 0; i < pointList.Count() && i < ScoreTable.Length; i++)
            {
               sb.AppendLine($"{i + 1}: <@{pointList[i].Key}> `{pointList[i].Value}pts`");
            }
            return sb.ToString();
        }

        public string GetProgress()
        {
            var isOver = QuizzProgress == NbTotalQuestions;
            return $"**[{QuizzProgress}/{NbTotalQuestions}]** {(isOver ? Resources.quizz_isOver : "")}";
        }

        public override string HeaderMessage
        {
            get => Timeout > 0
                ? String.Format(QuestionData.MessageWithTimeout, QuizzProgress, NbTotalQuestions, Timeout)
                : String.Format(QuestionData.Message, QuizzProgress, NbTotalQuestions);
        }

        public override OngoingQuizz GetNextProblem()
        {
            if(QuizzProgress >= NbTotalQuestions)
            {
                return null;
            }
            return new OngoingQuizz(Generator, Timeout, OnQuestionEnd, QuizzProgress + 1, NbTotalQuestions, PlayersAndPoints);
        }

    }
}
