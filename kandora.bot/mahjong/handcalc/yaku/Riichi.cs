
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Riichi : Yaku
    {

        public Riichi(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Riichi";
            this.tenhouId = 1;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
