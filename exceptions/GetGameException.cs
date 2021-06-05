using System;

namespace kandora.bot.exceptions
{
    public class GetGameException: Exception
    {
        public GetGameException(string message) : base(message)
        {
        }

        public GetGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GetGameException()
        {
        }
    }
}
