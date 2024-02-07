using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace kandora.bot.models
{
    public class Server
    {
        public Server(string id, string displayName, string leaderboardRoleId, string leaderboardName, int? leaderboardConfigId, int? leagueId)
        {
            Id = id;
            DisplayName = displayName;
            LeaderboardRoleId = leaderboardRoleId;
            LeaderboardName = leaderboardName;
            LeaderboardConfigId = leaderboardConfigId;
            LeagueId = leagueId;
            Users = new List<User>();
            Admins = new List<User>();
            Owners = new List<User>();
        }
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string LeaderboardRoleId { get; set; }
        public string LeaderboardName { get; set; }
        public int? LeaderboardConfigId { get; set; }
        public int? LeagueId { get; set; }

        public virtual List<User> Users { get; set; }
        public virtual List<User> Admins { get; set; }
        public virtual List<User> Owners { get; set; }
    }
}
