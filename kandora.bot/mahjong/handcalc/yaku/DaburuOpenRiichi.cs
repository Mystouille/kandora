
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Open DaburuRiichi
    //     
    public class DaburuOpenRiichi : Yaku
    {

        public DaburuOpenRiichi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Daburu Riichi (open)";
            this.han_closed = 3;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
