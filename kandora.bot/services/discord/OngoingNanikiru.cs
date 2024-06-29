using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.resources;
using kandora.bot.services.discord.problems;
using kandora.bot.services.nanikiru;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class OngoingNanikiru
    {
        public OngoingNanikiru(NanikiruGenerator generator, int timeout, Action<DiscordMessage> onQuestionEnd, int nbQuestions = 1)
        {
            this.Generator = generator;
            this.QuestionData = generator.GetNewQuestion();
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = 1;
            this.NbTotalQuestions = nbQuestions;
            this.OnQuestionEnd = onQuestionEnd;
        }

        private OngoingNanikiru(NanikiruGenerator generator, int timeout, Action<DiscordMessage> onQuestionEnd, int quizzProgress, int nbQuestions)
        {
            this.Generator = generator;
            this.QuestionData = generator.GetNewQuestion();
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = quizzProgress;
            this.NbTotalQuestions = nbQuestions;
            this.OnQuestionEnd = onQuestionEnd;
        }

        private NanikiruGenerator Generator { get; }
        public SingleChoiceQuestion QuestionData { get; }
        public int Timeout { get; }
        public DateTime StartTime { get; set; }
        private readonly Dictionary<ulong, ISet<ulong>> usersAnswers;
        public ulong Answer { get => QuestionData.AnswerEmoji.Id; }
        public ISet<ulong> Options { get => QuestionData.OptionEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public Dictionary<ulong, int> WinnersAndTiming { get; }
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

            var isWinner = usersAnswers.Count == 1 && usersAnswers[userId].Contains(Answer);

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

        private string getDisplayText(string textRaw, DiscordClient client)
        {
            var textithEmojis = textRaw;
            for (int n = 0; n<=9; n++)
            {
                foreach(var s in HandParser.SUIT_NAMES.ToList())
                {
                    var emojiStr = n.ToString() + s;
                    textithEmojis = textithEmojis.Replace(emojiStr, HandParser.GetEmojiCode(emojiStr, client));
                }
            }

            var compositeText = textithEmojis.Split('#');
            if(compositeText.Length != 0 && compositeText.Length % 3 == 0)
            {
                for(int i = 1; i < compositeText.Length; i+=3)
                {
                    var handText = compositeText[i].Split("+");
                    for(int h = 0; h < handText.Length; h++)
                    {
                        handText[h] = ""+HandParser.GetHandEmojiCodes(handText[h], client);
                    }
                    compositeText[i] = String.Join("" + HandParser.GetEmojiCode(Reactions.PLUS, client), handText);
                }
                textithEmojis = String.Join("\n", compositeText);
            }
            return textithEmojis;
        }

        public async Task OnProblemReaction(DiscordClient client, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if(client.CurrentUser.Id == user.Id)
            {
                return;
            }
            if(emoji.Id == DiscordEmoji.FromName(client, Reactions.EYES).Id){
                var sb = new StringBuilder();

                sb.AppendLine("||## "+getDisplayText(QuestionData.Ukeire, client)+"||");
                var winners = String.Join(", ",WinnersAndTiming.ToList().Select(winner => 
                    client.GetUserAsync(winner.Key).Result.Mention));
                sb.AppendLine("||"+ winners + " got it right! ||");
                var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
                await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
                await msg.DeleteAllReactionsAsync().ConfigureAwait(true);
                OnQuestionEnd.Invoke(msg);
            }
            if (!Options.Contains(emoji.Id))
            {
                return;
            }
            bool isWinner = ChangeUserAnswer(user.Id, emoji.Id, added);
        }

        public string GetProgress()
        {
            var isOver = QuizzProgress == NbTotalQuestions;
            return $"**[{QuizzProgress}/{NbTotalQuestions}]** {(isOver ? Resources.quizz_isOver : "")}";
        }

        public string HeaderMessage
        {
            get => Timeout > 0
                ? String.Format(QuestionData.MessageWithTimeout, QuizzProgress, NbTotalQuestions, Timeout)
                : String.Format(QuestionData.Message, QuizzProgress, NbTotalQuestions);
        }

        public OngoingNanikiru GetNextProblem()
        {
            if(QuizzProgress >= NbTotalQuestions)
            {
                return null;
            }
            return new OngoingNanikiru(Generator, Timeout, OnQuestionEnd, QuizzProgress + 1, NbTotalQuestions);
        }

    }
}
