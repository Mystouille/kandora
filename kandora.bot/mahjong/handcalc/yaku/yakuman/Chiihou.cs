
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

        public override void setAttributes()
        {
            this.tenhouId = 38;
            this.name = "Chiihou";
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
