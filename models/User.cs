using System;
using System.Collections.Generic;
using System.Text;

namespace Kandora
{
    class User
    {
        private string displayName;
        private int mahjsoulId;
        private bool isAdmin;

        public ulong Id { get; }
        public string DisplayName
        {
            set
            {
                displayName = value;
                UserDb.SetDiplayName(Id, value);
            }
            get
            {
                return displayName;
            }
        }
        public bool IsAdmin
        {
            set
            {
                isAdmin = value;
                UserDb.SetIsAdmin(Id, value);
            }
            get
            {
                return isAdmin;
            }
        }

        public int MahjsoulId
        {
            set
            {
                mahjsoulId = value;
                UserDb.SetMahjsoulId(Id, value);
            }
            get
            {
                return mahjsoulId;
            }
        }

        public User(ulong id, string displayName, int mahjsoulId, bool isAdmin)
        {
            this.Id = id;
            this.displayName = displayName;
            this.mahjsoulId = mahjsoulId;
            this.isAdmin = isAdmin;
        }
    }
}
