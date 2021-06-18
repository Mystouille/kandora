
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Pinfu : Yaku
    {

        public Pinfu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Pinfu";
            this.tenhouId = 7;
            this.nbHanOpen = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
