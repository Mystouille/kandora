
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Open DaburuRiichi
    //     
    public class DaburuRiichi : Yaku
    {

        public DaburuRiichi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.tenhou_id = 21;
            this.name = "Daburu Riichi";
            this.han_closed = 2;

            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
