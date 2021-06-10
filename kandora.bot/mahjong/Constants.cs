using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.mahjong
{
    public class Constants
    {
        public readonly static IEnumerable<int> TERMINAL_INDICES = new List<int> {
            0,
            8,
            9,
            17,
            18,
            26
        };

        public const int FIVE_RED_MAN = 16;
        public const int FIVE_RED_PIN = 52;
        public const int FIVE_RED_SOU = 88;

        public const int EAST = 27;

        public const int SOUTH = 28;

        public const int WEST = 29;

        public const int NORTH = 30;

        public const int HAKU = 31;

        public const int HATSU = 32;

        public const int CHUN = 33;

        public readonly static IEnumerable<int> WINDS = new List<int> {
            EAST,
            SOUTH,
            WEST,
            NORTH
        };

        public readonly static IEnumerable<int> HONOR_INDICES = new List<int> {
            EAST,
            SOUTH,
            WEST,
            NORTH,
            HAKU,
            HATSU,
            CHUN
        };

        public const int AGARI_STATE = -1;
    }
}
