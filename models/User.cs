using System;
using System.Collections.Generic;
using System.Text;

namespace Kandora
{
    class User
    {
        private string displayName;
        private int mahjsoulId;

        public int Id { get; }
        public ulong DiscordId { get; }
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

        public User(int id, string displayName, ulong discordId, int mahjsoulId)
        {
            this.Id = id;
            this.displayName = displayName;
            this.DiscordId = discordId;
            this.mahjsoulId = mahjsoulId;
        }
    }
}
