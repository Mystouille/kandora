namespace kandora.bot.mahjong
{
    using System.Collections.Generic;
    using System.Linq;

    public class Meld
    {

        public const string CHI = "chi";

        public const string PON = "pon";

        public const string KAN = "kan";

        public const string SHOUMINKAN = "shouminkan";

        public const string NUKI = "nuki";

        public int who;

        public List<int> tiles;

        public string type;

        public int fromWho;

        public int calledTile;

        public bool opened;

        public Meld(
            string meldType,
            List<int> tiles,
            bool opened,
            int calledTile,
            int who,
            int fromWho)
        {
            this.type = meldType;
            this.tiles = tiles != null ? tiles : new List<int>();
            this.opened = opened;
            this.calledTile = calledTile;
            this.who = who;
            this.fromWho = fromWho;
        }

        public override string ToString()
        {
            return $"Type: {this.type}, Tiles: {TilesConverter.ToString(this.tiles)} {this.tiles}";
        }

        public List<int> Tiles34
        {
            get
            {
                return (from x in this.tiles
                        select (x / 4)).ToList();
            }
        }
    }
    
}