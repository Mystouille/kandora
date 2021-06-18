
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Paarenchan : Yaku
    {
        private int count;

        public Paarenchan(int yaku_id) : base(yaku_id)
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

        public void setPaarenchanCount(int count)
        {
            this.nbHanOpen = 13 * count;
            this.nbHanClosed = 13 * count;
            this.count = count;
        }
    }
    
}
