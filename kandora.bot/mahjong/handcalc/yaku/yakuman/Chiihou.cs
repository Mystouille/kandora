
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Chiihou : Yaku
    {

        public Chiihou(int yaku_id) : base(yaku_id)
        {
        }

        public override void set_attributes()
        {
            this.tenhou_id = 38;
            this.name = "Chiihou";
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
