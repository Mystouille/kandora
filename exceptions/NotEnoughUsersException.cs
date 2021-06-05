using System;
namespace kandora.bot.exceptions
{
    public class NotEnoughUsersException: Exception
    {
        public NotEnoughUsersException(string message) : base(message)
        {
        }

        public NotEnoughUsersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotEnoughUsersException()
        {
        }
    }
}
