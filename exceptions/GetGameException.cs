using System;

namespace Kandora
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
