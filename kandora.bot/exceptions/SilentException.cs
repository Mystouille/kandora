using System;

namespace kandora.bot.exceptions
{
    public class SilentException : Exception
    {
        public SilentException(string message) : base(message)
        {
        }

        public SilentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SilentException()
        {
        }
    }
}
