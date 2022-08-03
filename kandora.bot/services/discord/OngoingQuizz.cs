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
    public class OngoingQuizz
    {
        public OngoingQuizz(IQuizzGenerator generator, int timeout, Action<DiscordMessage> onQuestionEnd, int nbQuestions = 1)
        {
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = 1;
            this.NbTotalQuestions = nbQuestions;
            this.Generator = generator;
            this.QuestionDataData = generator.GetNewQuestion();
            this.OnQuestionEnd = onQuestionEnd;
        }

        private OngoingQuizz(IQuizzGenerator generator, int timeout, Action<DiscordMessage> onQuestionEnd, int quizzProgress, int nbQuestions, Dictionary<ulong, int> playersAndPoints)
        {
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = playersAndPoints;
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = quizzProgress;
            this.NbTotalQuestions = nbQuestions;
            this.Generator = generator;
            this.QuestionDataData = generator.GetNewQuestion();
            this.OnQuestionEnd = onQuestionEnd;
        }

        private IQuizzGenerator Generator { get; }
        public MultipleChoicesQuestion QuestionDataData { get; }
        public int Timeout { get; }
        public DateTime StartTime { get; set; }
        private readonly Dictionary<ulong, ISet<ulong>> usersAnswers;
        public ISet<ulong> Answer { get => QuestionDataData.AnswerEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public ISet<ulong> Options { get => QuestionDataData.OptionEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public readonly int[] scoreTable = new int[] { 6, 4, 2, 1, 1 };
        public Dictionary<ulong, int> WinnersAndTiming { get; }
        public Dictionary<ulong,int> PlayersAndPoints { get; }
        public int NbTotalQuestions { get; }
        public int QuizzProgress { get; }
        public FileStream Image { get; }
        private Action<DiscordMessage> OnQuestionEnd { get; }

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

        public async Task OnProblemReaction(DiscordClient client, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
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
                return;
            }

            if (isWinner)
            {
                UpdateScores();
                var sb = new StringBuilder();
                sb.AppendLine(GetProgress());
                sb.AppendLine(String.Format(Resources.quizz_suddenDeathWinnerMessage,user.Mention));
                sb.AppendLine(String.Format(Resources.quizz_answer,string.Join("", QuestionDataData.AnswerEmojis)));
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
        public async void OnQuestionTimeout(DiscordMessage msg)
        {
            UpdateScores();
            var sb = new StringBuilder();
            sb.AppendLine(GetProgress());
            sb.AppendLine(String.Format(Resources.quizz_answer, string.Join("", QuestionDataData.AnswerEmojis)));

            if (WinnersAndTiming.Count == 0)
            {
                sb.AppendLine(Resources.quizz_timeoutNoWinnerMessage);
            }
            else
            {
                sb.AppendLine(GetFinalScore());
            }
            var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
            await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
            await msg.DeleteAllReactionsAsync().ConfigureAwait(true);
            OnQuestionEnd.Invoke(msg);
        }

        private void UpdateScores()
        {
            var timingList = WinnersAndTiming.ToList();
            timingList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            if (Timeout == 0)
            {
                if (timingList.Count == 0)
                {
                    throw new Exception("No winner found");
                }
                var userId = timingList[0].Key;
                AddPointsToUser(userId, 1);
            }
            else
            {
                for (int i = 0; i < scoreTable.Length && i < timingList.Count(); i++)
                {
                    AddPointsToUser(timingList[i].Key, scoreTable[i]);
                }
            }
        }

        private void AddPointsToUser(ulong userId, int nbPoints)
        {
            if (PlayersAndPoints.ContainsKey(userId))
            {
                PlayersAndPoints[userId] += nbPoints;
            }
            else
            {
                PlayersAndPoints.Add(userId, nbPoints);
            }
        }

        public string GetCurrentWinners()
        {
            if(Timeout == 0)
            {
                return "";
            }
            var sb = new StringBuilder();
            var timingList = WinnersAndTiming.ToList();
            timingList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            for (int i = 0; i < scoreTable.Length; i++)
            {
                if (i < timingList.Count())
                {
                    var userId = timingList[i].Key;
                    var totalScore = PlayersAndPoints.ContainsKey(userId) ? PlayersAndPoints[userId] : 0;
                    sb.AppendLine($"{i+1}: <@{timingList[i].Key}>`+{scoreTable[i]}pts` => {totalScore+scoreTable[i]}pts");
                }
                else
                {
                    sb.AppendLine($"{i+1}: ....");
                }
            }
            return sb.ToString();
        }

        private string GetFinalScore()
        {
            var sb = new StringBuilder();
            var pointList = PlayersAndPoints.ToList();
            pointList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            pointList.Reverse();
            for (int i = 0; i < pointList.Count() && i < scoreTable.Length; i++)
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

        public string HeaderMessage
        {
            get => Timeout > 0
                ? String.Format(QuestionDataData.MessageWithTimeout, QuizzProgress, NbTotalQuestions, Timeout)
                : String.Format(QuestionDataData.Message, QuizzProgress, NbTotalQuestions);
        }

        public OngoingQuizz GetNextProblem()
        {
            if(QuizzProgress >= NbTotalQuestions)
            {
                return null;
            }
            return new OngoingQuizz(Generator, Timeout, OnQuestionEnd, QuizzProgress + 1, NbTotalQuestions, PlayersAndPoints);
        }

    }
}
