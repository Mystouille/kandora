using System;

namespace Kandora
{
    public class GameNotSignedException: Exception
    {
        public GameNotSignedException(string message) : base(message)
        {
        }

        public GameNotSignedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GameNotSignedException()
        {
        }
    }
}
