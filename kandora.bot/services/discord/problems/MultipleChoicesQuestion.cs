
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;

namespace kandora.bot.services.discord.problems
{
    public class MultipleChoicesQuestion
    {
        public FileStream Image { get; set; }
        public IEnumerable<DiscordEmoji> OptionEmojis { get; set; }

        public MultipleChoicesQuestion(string message, string messageWithTimeout, FileStream image, IEnumerable<DiscordEmoji> optionEmojis, IEnumerable<DiscordEmoji> answerEmojis)
        {
            this.Message = message;
            this.MessageWithTimeout = messageWithTimeout;
            this.Image = image;
            this.OptionEmojis = optionEmojis;
            this.AnswerEmojis = answerEmojis;
        }
        public string Message { get; set; }
        public string MessageWithTimeout { get; set; }
        public IEnumerable<DiscordEmoji> AnswerEmojis { get; set; }

    }
}