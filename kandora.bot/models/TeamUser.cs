using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.models
{
    struct TeamUser
    {
        public TeamUser(string userId, int teamId, bool isCaptain)
        {
            this.userId = userId;
            this.teamId = teamId;
            this.isCaptain = isCaptain;
        }
        public string userId;
        public int teamId;
        public bool isCaptain;
    }
}
