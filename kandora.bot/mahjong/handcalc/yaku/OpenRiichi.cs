
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

        public override void setAttributes()
        {
            this.name = "Open Riichi";
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
