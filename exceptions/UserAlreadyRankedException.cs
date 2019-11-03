using System;

namespace Kandora
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
