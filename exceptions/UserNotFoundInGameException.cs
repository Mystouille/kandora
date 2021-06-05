
using System;

namespace kandora.bot.exceptions
{
    public class UserNotFoundInGameException : Exception 
    {
        public UserNotFoundInGameException(string message) : base(message)
        {
        }

        public UserNotFoundInGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserNotFoundInGameException()
        {
        }
    }
}
