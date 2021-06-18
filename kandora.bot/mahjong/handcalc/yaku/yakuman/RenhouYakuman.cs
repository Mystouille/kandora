
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class RenhouYakuman : Yaku
    {

        public RenhouYakuman(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Renhou (Yakuman)";
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
