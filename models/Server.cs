using System;
using System.Collections.Generic;
using System.Text;

namespace kandora.bot.models
{
    public class Server
    {
        public Server(string id, string displayName, string targetChannelId)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.TargetChannelId = targetChannelId;
        }
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string TargetChannelId { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<User> Admins { get; set; }
        public virtual ICollection<User> Owners { get; set; }
    }
}
