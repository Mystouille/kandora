using System;

namespace kandora.bot.exceptions
{
    public class UserNotRegisteredException : Exception
    {
        public UserNotRegisteredException(string userId) : base(getMessage(userId))
        {
        }

        public UserNotRegisteredException(string userId, Exception innerException) : base(getMessage(userId), innerException)
        {
        }

        public UserNotRegisteredException(): base (getMessage("User"))
        {
        }

        private static string getMessage(string userId)
        {
            return $"<@{userId}> need to be registered to record a game";
        }
    }
}
