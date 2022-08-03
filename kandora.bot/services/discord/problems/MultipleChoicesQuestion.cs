
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace kandora.bot.services.discord.problems
{
    public class MultipleChoicesQuestion
    {
        public MultipleChoicesQuestion(string message, string messageWithTimeout, FileStream image, IEnumerable<DiscordEmoji> optionEmojis, IEnumerable<DiscordEmoji> answerEmojis)
        {
            this.Message = message;
            this.MessageWithTimeout = messageWithTimeout;
            this.Image = image;
            this.OptionEmojis = optionEmojis;
            this.AnswerEmojis = answerEmojis;
        }
        public string Message { get; }
        public string MessageWithTimeout { get; }
        public FileStream Image { get; }
        public IEnumerable<DiscordEmoji> OptionEmojis { get; }
        public IEnumerable<DiscordEmoji> AnswerEmojis { get; }

    }
}