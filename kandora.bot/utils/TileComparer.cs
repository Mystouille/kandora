using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.utils
{
    public class TileComparer : Comparer<string>
    {
        // Compares by Length, Height, and Width.
        public override int Compare(string x, string y)
        {
            if (x.Last().CompareTo(y.Last()) == 0)
            {
                return x.First().CompareTo(y.First());
            }
            else
            {
                return x.Last().CompareTo(y.Last());
            }
        }
    }
}
