
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Daisangen : Yaku
    {

        public Daisangen(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Daisangen";
            this.tenhou_id = 39;
            this.han_open = 13;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var dragons = new int[] { Constants.CHUN, Constants.HAKU, Constants.HATSU };
            var count = 0;
            foreach(var group in hand)
            {
                if(Utils.is_pon_or_kan(group) && dragons.Contains(group[0]))
                {
                    count++;
                }
            }
            return count == 3;
        }
    }
    
}
