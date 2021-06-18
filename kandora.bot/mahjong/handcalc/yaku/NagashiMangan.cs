
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class NagashiMangan : Yaku
    {

        public NagashiMangan(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Nagashi Mangan";
            this.nbHanOpen = 5;
            this.nbHanClosed = 5;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
