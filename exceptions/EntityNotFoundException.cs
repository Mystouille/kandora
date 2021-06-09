using System;

namespace kandora.bot.exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, string entityId) : base(getMessage(entityName, entityId))
        {
        }

        public EntityNotFoundException(string entityName, string entityId, Exception innerException) : base(getMessage(entityName, entityId), innerException)
        {
        }


        private static string getMessage(string entityName, string entityId)
        {
            return $"SQL Entity not found: {entityName}, id: {entityId}";
        }
    }
}
