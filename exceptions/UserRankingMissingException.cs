using System;

namespace Kandora
{
    public class UserRankingMissingException: Exception
    {
        public UserRankingMissingException(string message) : base(message)
        {
        }

        public UserRankingMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserRankingMissingException()
        {
        }
    }
}
