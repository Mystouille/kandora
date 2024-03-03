using System;
using System.Collections.Generic;

namespace kandora.bot.models
{
    public partial class User
    {
        public User(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public string MahjsoulFriendId { get; set; }
        public string MahjsoulUserId { get; set; }
        public string DisplayName { get; set; }
        public string MahjsoulName { get; set; }
        public string TenhouName { get; set; }
        public int RiichiCityId { get; set; }
        public int RiichiCitySecondaryId { get; set; }
        
        public string RiichiCityName { get; set; }
    }
}
