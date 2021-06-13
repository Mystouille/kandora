
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class OpenRiichi : Yaku
    {

        public OpenRiichi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Open Riichi";
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
