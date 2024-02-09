using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.models
{
    struct Team
    {
        public Team(int teamId, string name, string fancyName)
        {
            this.teamId = teamId;
            this.name = name;
            this.fancyName = fancyName;
        }
        public int teamId;
        public string name;
        public string fancyName;
    }
}
