
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Daisuushii : Yaku
    {

        public Daisuushii(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Daisuushii";
            this.tenhou_id = 49;
            this.han_open = 26;
            this.han_closed = 26;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var pons = hand.Where(x => Utils.is_pon_or_kan(x));
            if (pons.Count() != 4)
            {
                return false;
            }
            var nbWindPon = 0;
            var winds = new int[] { Constants.EAST, Constants.WEST, Constants.SOUTH, Constants.NORTH };
            foreach(var group in hand)
            {
                if (Utils.is_pon_or_kan(group) && winds.Contains(group[0]))
                {
                    nbWindPon++;
                }
            }
            return nbWindPon == 4;
        }
    }
    
}
