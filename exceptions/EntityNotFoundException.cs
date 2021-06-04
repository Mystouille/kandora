using System;

namespace Kandora
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName) : base(getMessage(entityName))
        {
        }

        public EntityNotFoundException(string entityName, Exception innerException) : base(getMessage(entityName), innerException)
        {
        }


        private static string getMessage(string entityName)
        {
            return $"Entity not found {entityName}";
        }
    }
}
