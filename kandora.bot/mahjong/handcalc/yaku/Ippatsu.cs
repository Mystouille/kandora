
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Ippatsu : Yaku
    {

        public Ippatsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Ippatsu";
            this.tenhouId = 2;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
