
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

        public override void setAttributes()
        {
            this.name = "Daburu Riichi (open)";
            this.nbHanClosed = 3;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
