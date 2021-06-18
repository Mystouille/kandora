
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sashikomi : Yaku
    {

        public Sashikomi(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Sashikomi";
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
