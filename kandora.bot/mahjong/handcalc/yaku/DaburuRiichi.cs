
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

        public override void setAttributes()
        {
            this.tenhouId = 21;
            this.name = "Daburu Riichi";
            this.nbHanClosed = 2;

            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
