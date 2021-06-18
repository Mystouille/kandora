
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class DaburuKokushi : Yaku
    {

        public DaburuKokushi(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Daburu Kokushi";
            this.tenhouId = 48;
            this.nbHanClosed = 26;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
