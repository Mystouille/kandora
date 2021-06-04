using System;

namespace Kandora
{
    public class GameNotSignedOffException: Exception
    {
        public GameNotSignedOffException(string message) : base(message)
        {
        }

        public GameNotSignedOffException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GameNotSignedOffException()
        {
        }
    }
}
