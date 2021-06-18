
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo on last draw
    //     
    public class Haitei : Yaku
    {

        public Haitei(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Haitei Raoyue";
            this.tenhouId = 5;
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
