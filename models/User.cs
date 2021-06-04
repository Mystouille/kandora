using System;
using System.Collections.Generic;

namespace kandora.bot.models
{
    public partial class User
    {
        public User()
        {
        }
        public User(string id, string displayName, string mahjsoulId, string tenhouId) : this()
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.MahjsoulId = mahjsoulId;
            this.TenhouId = tenhouId;
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string MahjsoulId { get; set; }
        public string TenhouId { get; set; }
    }
}
