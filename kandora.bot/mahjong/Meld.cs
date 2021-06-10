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

        public int from_who;

        public int called_tile;

        public bool opened;

        public Meld(
            string meld_type,
            List<int> tiles,
            bool opened,
            int called_tile,
            int who,
            int from_who)
        {
            this.type = meld_type;
            this.tiles = tiles != null ? tiles : new List<int>();
            this.opened = opened;
            this.called_tile = called_tile;
            this.who = who;
            this.from_who = from_who;
        }

        public override string ToString()
        {
            return $"Type: {this.type}, Tiles: {TilesConverter.to_one_line_string(this.tiles)} {this.tiles}";
        }

        public List<int> tiles_34
        {
            get
            {
                return (from x in this.tiles
                        select (x / 4)).ToList();
            }
        }
    }
    
}