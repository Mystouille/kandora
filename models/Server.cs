using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace kandora.bot.models
{
    public class Server
    {
        public Server(string id, string displayName, string leagueRoleId, string leagueName, string targetChannelId)
        {
            Id = id;
            DisplayName = displayName;
            LeagueRoleId = leagueRoleId;
            LeagueName = leagueName;
            TargetChannelId = targetChannelId;
            Users = new List<User>();
            Admins = new List<User>();
            Owners = new List<User>();
        }
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string LeagueRoleId { get; set; }
        public string LeagueName { get; set; }
        public string TargetChannelId { get; set; }

        public virtual List<User> Users { get; set; }
        public virtual List<User> Admins { get; set; }
        public virtual List<User> Owners { get; set; }
    }
}
