
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Robbing a kan
    //     
    public class Chankan : Yaku
    {

        public Chankan(int yaku_id): base(yaku_id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Chankan";
            this.tenhouId = 3;
            this.nbHanOpen = 1;
            this.nbHanClosed = 1;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
