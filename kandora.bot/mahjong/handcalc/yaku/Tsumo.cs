
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Tsumo : Yaku
    {

        public Tsumo(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Menzen Tsumo";
            this.tenhouId = 0;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
