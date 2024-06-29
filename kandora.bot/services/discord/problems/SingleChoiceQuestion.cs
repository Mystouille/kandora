
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace kandora.bot.services.discord.problems
{
    public class SingleChoiceQuestion
    {
        public SingleChoiceQuestion(FileStream image, IEnumerable<DiscordEmoji> optionEmojis, DiscordEmoji answerEmoji, string explanation, string ukeire, string source)
        {
            this.Image = image;
            this.OptionEmojis = optionEmojis;
            this.AnswerEmoji = answerEmoji;
            this.Explanation = explanation;
            this.Ukeire = ukeire;
            this.Source = source;
        }
        public string Message { get; }
        public string Explanation { get; }
        public string Ukeire { get; }
        public string Source { get; }
        public string MessageWithTimeout { get; }
        public FileStream Image { get; }
        public IEnumerable<DiscordEmoji> OptionEmojis { get; }
        public DiscordEmoji AnswerEmoji { get; }

    }
}