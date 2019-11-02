using System;
using System.Collections.Generic;
using System.Text;

namespace Kandora
{
    class User
    {
        public int id { get; }
        public string displayName {
            set {
                displayName = value;
                UserDb.setDiplayName(id, value);
            }
            get
            {
                return displayName;
            }
        }
        public string uniqueName {
            set
            {
                uniqueName = value;
                UserDb.setUniqueName(id, value);
            }
            get
            {
                return uniqueName;
            }
        }
        public string mahjsoulId {
            set
            {
                mahjsoulId = value;
                UserDb.setMahjsoulId(id, value);
            }
            get
            {
                return mahjsoulId;
            }
        }

        public User(int id, string displayName, string uniqueName, string mahjsoulId)
        {
            this.id = id;
            this.displayName = displayName;
            this.uniqueName = uniqueName;
            this.mahjsoulId = mahjsoulId;
        }
    }
}
