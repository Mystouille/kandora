using System;

namespace kandora.bot.exceptions
{
    public class UserAlreadyRankedException: Exception
    {
        public UserAlreadyRankedException(string message) : base(message)
        {
        }

        public UserAlreadyRankedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserAlreadyRankedException()
        {
        }
    }
}
