using System;

namespace Kandora
{
    public class ServerNotRegisteredException : Exception
    {
        public ServerNotRegisteredException() : base(getMessage())
        {
        }

        public ServerNotRegisteredException(Exception innerException) : base(getMessage(), innerException)
        {
        }


        private static string getMessage()
        {
            return $"Whoops, for some reason this server is not registered.";
        }
    }
}
