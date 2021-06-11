
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Tenhou : Yaku
    {

        public Tenhou(object yaku_id = null)
        {
        }

        public override void set_attributes()
        {
            this.tenhou_id = 37;
            this.name = "Tenhou";
            this.han_open = 0;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
