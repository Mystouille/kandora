using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.mahjong;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class OngoingProblem
    {
        public OngoingProblem(ISet<int> answer, ISet<ulong> options = null, bool hasTimer = false)
        {
            Answer = answer;
            this.answers = new Dictionary<ulong, ISet<int>>();
            if (options == null)
            {
                Options = new HashSet<ulong>();
            }
            else
            {
                this.Options = options;
            }
            this.winnersAndTiming = new List<(ulong, int)>();
            this.hasTimer = hasTimer;
            if (hasTimer)
            {
                this.startTime = DateTime.Now;
            }

        }

        public bool hasTimer;
        public DateTime startTime;
        private Dictionary<ulong, ISet<int>> answers;
        public ISet<int> Answer { get; }
        public ISet<ulong> Options { get; }
        public List<(ulong, int)> winnersAndTiming { get; }
        public bool ChangeUserAnswer(ulong userId, int answer, bool isAdd)
        {
            if (isAdd)
            {
                if (!answers.ContainsKey(userId))
                {
                    answers.Add(userId, new HashSet<int>());
                }
                answers[userId].Add(answer);
            }
            else
            {
                if (answers.ContainsKey(userId))
                {
                    answers[userId].Remove(answer);
                }
            }

            if (!answers.ContainsKey(userId))
            {
                return false;
            }

            var isWinner = answers[userId].Intersect(Answer).Count() == Answer.Count && answers[userId].Count == Answer.Count;

            if (isWinner)
            {
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                winnersAndTiming.Add((userId, (int)duration.TotalMilliseconds));
            }
            else
            {
                winnersAndTiming.RemoveAll(x => x.Item1 == userId);
            }
            return isWinner;
        }

        static int fromEmojiToTile34(DiscordEmoji emoji)
        {
            var emojiSplit = emoji.ToString().Split(':');
            var array = TilesConverter.FromStringTo34Count(emojiSplit[1]);
            for(int i = 0; i< array.Length; i++)
            {
                if (array[i] > 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public async Task<bool> OnProblemReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if (!Options.Contains(emoji.Id))
            {
                return false;
            }
            var tile34 = fromEmojiToTile34(emoji);
            bool isWinner = ChangeUserAnswer(user.Id, tile34, added);

            if (hasTimer)
            {
                return false;
            }

            if (isWinner)
            {
                var answer136 = TilesConverter.From34IxdHandTo136(new List<List<int>> { Answer.ToList() });
                var answerStr = TilesConverter.ToString(answer136);
                var answerEmoji = HandParser.GetHandEmojiCodes(answerStr, sender);
                var sb = new StringBuilder();
                sb.AppendLine($"{user.Mention} got this one first!");
                sb.AppendLine($"The answer was: {string.Join("", answerEmoji)}");
                await msg.RespondAsync(sb.ToString());
                return true;
            }
            return false;
        }

        public async Task OnProblemTimeout(DiscordClient sender, DiscordMessage msg)
        {
            var answer136 = TilesConverter.From34IxdHandTo136(new List<List<int>> { Answer.ToList() });
            var answerStr = TilesConverter.ToString(answer136);
            var answerEmoji = HandParser.GetHandEmojiCodes(answerStr, sender);
            var sb = new StringBuilder();
            sb.AppendLine($"The answer was: {string.Join("", answerEmoji)}");

            if(winnersAndTiming.Count == 0)
            {
                sb.AppendLine($"Looks like this was a bit too hard for you!");
            }
            else
            {
                sb.AppendLine($"Top 3:");
                for (int i = 0; i < 3 && i < winnersAndTiming.Count; i++)
                {
                    sb.AppendLine($"<@{winnersAndTiming[i].Item1}>: {(float)(winnersAndTiming[i].Item2) / 1000}s");
                }
            }
            await msg.RespondAsync(sb.ToString());
        }

    }
}
