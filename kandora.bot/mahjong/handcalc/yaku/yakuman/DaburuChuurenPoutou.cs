
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class DaburuChuurenPoutou : Yaku
    {

        public DaburuChuurenPoutou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Daburu Chuuren Poutou";
            this.tenhouId = 46;
            this.nbHanClosed = 26;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
