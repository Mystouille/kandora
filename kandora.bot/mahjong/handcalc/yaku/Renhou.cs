
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Renhou : Yaku
    {

        public Renhou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Renhou";
            this.tenhouId = 36;
            this.nbHanClosed = 5;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
