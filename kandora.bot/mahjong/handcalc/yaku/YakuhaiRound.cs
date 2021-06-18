
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

        public override void setAttributes()
        {
            this.name = "YakuhaiPlace (wind of place)";
            this.tenhouId = 11;
            this.nbHanOpen = 1;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
