
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Tenhou : Yaku
    {

        public Tenhou(int yaku_id) : base(yaku_id)
        {
        }

        public override void setAttributes()
        {
            this.tenhouId = 37;
            this.name = "Tenhou";
            this.nbHanOpen = 0;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
