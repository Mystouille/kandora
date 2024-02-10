using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.models
{
    struct TeamUser
    {
        public TeamUser(int id, string userId, int teamId, bool isCaptain)
        {
            this.id = id;
            this.userId = userId;
            this.teamId = teamId;
            this.isCaptain = isCaptain;
        }
        public int id;
        public string userId;
        public int teamId;
        public bool isCaptain;
    }
}
