using System;

namespace Kandora
{
    public class DbConnectionException: Exception
    {
        public DbConnectionException(string message) : base(message)
        {
        }

        public DbConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DbConnectionException()
        {
        }
    }
}
