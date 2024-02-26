using System;

namespace kandora.bot.models
{
    internal class League
    {
        public League(int id, string displayName, string serverId, bool isOngoing, DateTime? finalsCutoffDate)
        {
            Id = id;
            DisplayName = displayName;
            ServerId = serverId;
            IsOngoing = isOngoing;
            FinalsCutoffDate = finalsCutoffDate;
        }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ServerId { get; set; }
        public bool IsOngoing { get; set; }

        public DateTime? FinalsCutoffDate { get; set; }
    }
}
