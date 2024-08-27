using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.resources;
using kandora.bot.services.discord.problems;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class OngoingNanikiru : OngoingProblem
    {
        public OngoingNanikiru(NanikiruGenerator generator, DiscordClient client, int timeout, int nbQuestions = 1)
        {
            this.Generator = generator;
            this.SpecificQuestionData = generator.GetNewQuestion(); ;
            this.QuestionData = SpecificQuestionData;
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.LosersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = 1;
            this.NbTotalQuestions = nbQuestions;
            this.ScoreTable = new int[] {};
            this.Client = client;
        }

        private OngoingNanikiru(NanikiruGenerator generator, DiscordClient client, int timeout, Action<DiscordMessage> onQuestionEnd, int quizzProgress, int nbQuestions)
        {
            this.Generator = generator;
            this.SpecificQuestionData = generator.GetNewQuestion(); ;
            this.QuestionData = SpecificQuestionData;
            this.WinnersAndTiming = new Dictionary<ulong, int>();
            this.LosersAndTiming = new Dictionary<ulong, int>();
            this.PlayersAndPoints = new Dictionary<ulong, int>();
            this.usersAnswers = new Dictionary<ulong, ISet<ulong>>();
            this.Timeout = timeout;
            ResetTimer();
            this.QuizzProgress = quizzProgress;
            this.NbTotalQuestions = nbQuestions;
            this.OnQuestionEnd = onQuestionEnd;
            this.ScoreTable = new int[] {};
            this.Client = client;
        }

        private NanikiruGenerator Generator { get; }
        private readonly Dictionary<ulong, ISet<ulong>> usersAnswers;
        private NanikiruQuestion SpecificQuestionData { get; set; }
        public List<ulong> CorrectAnswers { get => QuestionData.AnswerEmojis.Select(x=>x.Id).ToList(); }
        public ISet<ulong> Options { get => QuestionData.OptionEmojis.Select(emoji => emoji.Id).ToHashSet(); }
        public int QuizzProgress { get; }
        public FileStream Image { get; }


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
            var correctUserAnswer = usersAnswers[userId].Where(userAnswer => CorrectAnswers.Contains(userAnswer)).ToList();
            var isWinner = correctUserAnswer.Count() == CorrectAnswers.Count && usersAnswers[userId].Count == CorrectAnswers.Count;

            var endTime = DateTime.Now;
            var duration = endTime - StartTime;
            var millisecondDuration = (int)duration.TotalMilliseconds;
            if (isWinner)
            {
                if (WinnersAndTiming.ContainsKey(userId))
                {
                    WinnersAndTiming[userId] = millisecondDuration;
                }
                else
                {
                    WinnersAndTiming.Add(userId, millisecondDuration);
                }
                LosersAndTiming.Remove(userId);
            }
            else
            {
                if (LosersAndTiming.ContainsKey(userId))
                {
                    LosersAndTiming[userId] = millisecondDuration;
                }
                else
                {
                    LosersAndTiming.Add(userId, millisecondDuration);
                }
                WinnersAndTiming.Remove(userId);
            }
            return isWinner;
        }

        private string getDisplayText(string textRaw, DiscordClient client, string prefix = "", bool spoiler = false)
        {
            var compositeText = textRaw.Split('#');
            var plusSeparator = "➕";
            for (int i = 0; i < compositeText.Length; i ++)
            {
                if ((i + 2) % 3 == 0)
                {
                    var handText = compositeText[i].Split("+");
                    List<string> emojiHand = new List<string>();
                    foreach (var part in handText)
                    {
                        emojiHand.Add(HandParser.GetHandEmojiString(part, client));
                    }
                    compositeText[i] = "# " + String.Join(plusSeparator, emojiHand);
                } else
                {
                    var textithEmojis = compositeText[i];
                    for (int n = 0; n <= 9; n++)
                    {
                        foreach (var s in HandParser.SUIT_NAMES.ToList())
                        {
                            if (s == 'z' && (n > 4 || n == 0))
                            {
                                continue;
                            }
                            var emojiStr = n.ToString() + s;
                            textithEmojis = textithEmojis.Replace(emojiStr, HandParser.GetEmojiCode(emojiStr, client));
                        }
                    }
                    compositeText[i] = (textithEmojis.Length>0 ? prefix : "") + textithEmojis;

                }
            }
            var finalText = String.Join("\n", compositeText);
            if (spoiler)
            {
                finalText = "||" + finalText + "||";
            }
            return finalText;
        }

        private async Task DisplayAnswer(DiscordMessage msg)
        {
            UpdateScores();
            var sb = new StringBuilder();
            sb.AppendLine(Resources.quizz_nanikiru_problem_over);
            sb.AppendLine(HeaderMessage);
            var mb = new DiscordMessageBuilder().WithContent(sb.ToString());
            await msg.ModifyAsync(mb, attachments: msg.Attachments).ConfigureAwait(true);
            await msg.DeleteAllReactionsAsync().ConfigureAwait(true);

            sb = new StringBuilder();
            sb.AppendLine("# " + Resources.quizz_nanikiru_answer + " ||" + SpecificQuestionData.AnswerEmojis.Select(x=>x.ToString()).Aggregate((x,y)=>x+""+y) +"||");

            var winners = String.Join(", ", WinnersAndTiming.ToList().Select(winner =>
                Client.GetUserAsync(winner.Key).Result.Mention));
            var losers = String.Join(", ", LosersAndTiming.ToList().Select(losers =>
                Client.GetUserAsync(losers.Key).Result.Mention));
            if (winners.Length > 0)
            {
                sb.AppendLine(String.Format(Resources.quizz_nanikiru_winners, winners));
            }
            if (losers.Length > 0)
            {
                sb.AppendLine(String.Format(Resources.quizz_nanikiru_losers, losers));
            }

            sb.AppendLine(getDisplayText(SpecificQuestionData.Ukeire, Client, "### ", spoiler: true));
            sb.AppendLine(getDisplayText(SpecificQuestionData.Explanation, Client, "## ", spoiler: true));

            sb.AppendLine("### " + SpecificQuestionData.Source);


            await msg.RespondAsync(sb.ToString()).ConfigureAwait(true);
        }

        public override async Task OnProblemReaction(DiscordClient client, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if(client.CurrentUser.Id == user.Id)
            {
                return;
            }
            if(emoji.Id == DiscordEmoji.FromName(client, Reactions.EYES).Id){
                await DisplayAnswer(msg).ConfigureAwait(true);
                OnQuestionEnd.Invoke(msg);
            }
            else if (Options.Contains(emoji.Id))
            {
                ChangeUserAnswer(user.Id, emoji.Id, added);
            }
        }

        public override async void OnQuestionTimeout(DiscordMessage msg)
        {
            UpdateScores();
            await DisplayAnswer(msg).ConfigureAwait(true);
            OnQuestionEnd.Invoke(msg);
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

        public override OngoingNanikiru GetNextProblem()
        {
            if(QuizzProgress >= NbTotalQuestions)
            {
                return null;
            }
            return new OngoingNanikiru(Generator, Client, Timeout, OnQuestionEnd, QuizzProgress + 1, NbTotalQuestions);
        }

    }
}
