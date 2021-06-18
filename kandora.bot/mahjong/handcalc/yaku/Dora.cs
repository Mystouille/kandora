
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Dora : Yaku
    {

        public Dora(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.tenhouId = 52;
            this.name = "Dora";
            this.nbHanOpen = 1;
            this.nbHanClosed = 1;
            this.isYakuman = false;
        }
        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }

        public override string ToString()
        {
            return $"{this.name} {this.nbHanClosed}";
        }
    }
    
}
