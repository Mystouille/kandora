
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace kandora.bot.services.discord.problems
{
    public class NanikiruQuestion : MultipleChoicesQuestion
    {
        public NanikiruQuestion(string message, string messageWithTimeout, FileStream image, IEnumerable<DiscordEmoji> optionEmojis, DiscordEmoji answerEmoji, string explanation, string ukeire, string source)
            : base(message, messageWithTimeout, image, optionEmojis, new List<DiscordEmoji> { answerEmoji })
        {
            this.Image = image;
            this.Explanation = explanation;
            this.Ukeire = ukeire;
            this.Source = source;
            this.AnswerEmoji = answerEmoji;
        }

        public string Explanation { get; }
        public string Ukeire { get; }
        public string Source { get; }
        public DiscordEmoji AnswerEmoji { get; }

    }
}