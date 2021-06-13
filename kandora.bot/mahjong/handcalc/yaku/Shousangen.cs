
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Shousangen : Yaku
    {

        public Shousangen(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Shousangen";
            this.tenhou_id = 30;
            this.han_open = 2;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var dragons = new int[] { Constants.CHUN, Constants.HAKU, Constants.HATSU };
            var count = 0;
            foreach(var group in hand)
            {
                if((Utils.is_pair(group) || Utils.is_pon_or_kan(group)) && dragons.Contains(group[0]))
                {
                    count++;
                }
            }
            return count == 3;
        }
    }
    
}
