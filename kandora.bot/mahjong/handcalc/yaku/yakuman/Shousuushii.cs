
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Shousuushii : Yaku
    {

        public Shousuushii(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Shousuushii";
            this.tenhou_id = 50;
            this.han_open = 13;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var pons = hand.Where(x => Utils.is_pon_or_kan(x));
            if (pons.Count() < 3)
            {
                return false;
            }
            var nbWindPon = 0;
            var nbWindPair = 0;
            var winds = new int[] { Constants.EAST, Constants.WEST, Constants.SOUTH, Constants.NORTH };
            foreach(var group in hand)
            {
                if (Utils.is_pon_or_kan(group) && winds.Contains(group[0]))
                {
                    nbWindPon++;
                }
                if (Utils.is_pair(group) && winds.Contains(group[0]))
                {
                    nbWindPair++;
                }
            }
            return nbWindPon == 3 && nbWindPair == 1;
        }
    }
    
}
