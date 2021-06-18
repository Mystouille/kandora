
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Rinshan : Yaku
    {

        public Rinshan(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Rinshan Kaihou";
            this.tenhouId = 4;
            this.nbHanOpen = 1;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
