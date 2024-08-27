
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace kandora.bot.services.discord.problems
{
    public class NanikiruQuestion : MultipleChoicesQuestion
    {
        public NanikiruQuestion(string message, string messageWithTimeout, FileStream image, IEnumerable<DiscordEmoji> optionEmojis, List<DiscordEmoji> answerEmojis, string explanation, string ukeire, string source)
            : base(message, messageWithTimeout, image, optionEmojis, answerEmojis)
        {
            this.Image = image;
            this.Explanation = explanation;
            this.Ukeire = ukeire;
            this.Source = source;
            this.AnswerEmojis = answerEmojis;
        }

        public string Explanation { get; }
        public string Ukeire { get; }
        public string Source { get; }

    }
}