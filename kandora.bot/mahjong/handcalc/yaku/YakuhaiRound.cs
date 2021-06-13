
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class YakuhaiRound : Yaku
    {

        public YakuhaiRound(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "YakuhaiPlace (wind of place)";
            this.tenhou_id = 11;
            this.han_open = 1;
            this.han_closed = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
