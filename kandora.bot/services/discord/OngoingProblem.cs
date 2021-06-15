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
        public OngoingProblem(ISet<int> answer, ISet<ulong> options = null)
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
        }

        private Dictionary<ulong, ISet<int>> answers;
        public ISet<int> Answer { get; }
        public ISet<ulong> Options { get; }
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

            var isWinner = answers[userId].Intersect(Answer).Count() == Answer.Count();
            return isWinner;
        }

        static int fromEmojiToTile34(DiscordEmoji emoji)
        {
            var emojiSplit = emoji.ToString().Split(':');
            var array = TilesConverter.one_line_string_to_34_array(emojiSplit[1]);
            for(int i = 0; i< array.Length; i++)
            {
                if (array[i] > 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public async static Task OnProblemReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            var kanContext = KandoraContext.Instance;
            var msgId = msg.Id;
            var problem = kanContext.OngoingProblems[msgId];

            if (!problem.Options.Contains(emoji.Id))
            {
                return;
            }
            var tile34 = fromEmojiToTile34(emoji);
            bool isWinner = problem.ChangeUserAnswer(user.Id, tile34, added);

            if (isWinner)
            {
                kanContext.OngoingProblems.Remove(msgId);
                var answer136 = TilesConverter.from_34_indices_to_136_array(new List<List<int>> { problem.Answer.ToList() });
                var answerStr = TilesConverter.to_one_line_string(answer136);
                var answerEmoji = HandParser.GetHandEmojiCodes(answerStr, sender);
                var sb = new StringBuilder();
                sb.AppendLine($"{user.Mention} got this one first!");
                sb.AppendLine($"The answer was: {string.Join("", answerEmoji)}");
                await msg.RespondAsync(sb.ToString());
            }
        }
    }
}
