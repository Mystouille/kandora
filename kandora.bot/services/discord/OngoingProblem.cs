using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.services.discord.problems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public abstract class OngoingProblem
    {
        public DateTime StartTime { get; set; }
        public int Timeout { get; set; }
        public int NbTotalQuestions { get; set; }
        public void ResetTimer()
        {
            StartTime = DateTime.Now;
        }
        public abstract string HeaderMessage
        {
            get;
        }
        public Action<DiscordMessage> OnQuestionEnd { get; set; }
        public Dictionary<ulong, int> WinnersAndTiming { get; set; }
        public Dictionary<ulong, int> LosersAndTiming { get; set; }
        public Dictionary<ulong, int> PlayersAndPoints { get; set; }

        public abstract void OnQuestionTimeout(DiscordMessage msg);
        public int[] ScoreTable { get; set; }
        protected DiscordClient Client { get; set; }

        public MultipleChoicesQuestion QuestionData { get; set; }

        public abstract Task OnProblemReaction(DiscordClient client, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added);
        public abstract OngoingProblem GetNextProblem();
        public string GetCurrentWinners()
        {
            if (Timeout == 0)
            {
                return "";
            }
            var sb = new StringBuilder();
            var timingList = WinnersAndTiming.ToList();
            timingList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            for (int i = 0; i < ScoreTable.Length; i++)
            {
                if (i < timingList.Count())
                {
                    var userId = timingList[i].Key;
                    var totalScore = PlayersAndPoints.ContainsKey(userId) ? PlayersAndPoints[userId] : 0;
                    sb.AppendLine($"{i + 1}: <@{timingList[i].Key}>`+{ScoreTable[i]}pts` ({Math.Round((float)(timingList[i].Value) / 1000, 1)}s)");
                }
                else
                {
                    sb.AppendLine($"{i + 1}: ....");
                }
            }
            return sb.ToString();
        }



        protected void AddPointsToUser(ulong userId, int nbPoints)
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

        protected void UpdateScores()
        {
            var timingList = WinnersAndTiming.ToList();
            timingList.Sort((x, y) => {
                return y.Value.CompareTo(x.Value);
            });
            if (Timeout == 0)
            {
                if (timingList.Count > 0)
                {
                    var userId = timingList[0].Key;
                    AddPointsToUser(userId, 1);
                }
            }
            else
            {
                for (int i = 0; i < ScoreTable.Length && i < timingList.Count(); i++)
                {
                    AddPointsToUser(timingList[i].Key, ScoreTable[i]);
                }
            }
        }


    }
}
