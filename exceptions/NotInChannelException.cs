using System;

namespace kandora.bot.exceptions
{
    public class NotInChannelException : Exception
    {
        public NotInChannelException() : base(getMessage())
        {
        }

        public NotInChannelException(Exception innerException) : base(getMessage(), innerException)
        {
        }


        private static string getMessage()
        {
            return $"You must be in a text channel for that command.";
        }
    }
}
