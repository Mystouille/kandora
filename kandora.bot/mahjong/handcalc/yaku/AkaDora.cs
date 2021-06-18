
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku
{

    // 
    //     Red five
    //     
    public class AkaDora : Yaku
    {

        public AkaDora(int yaku_id) : base(yaku_id)
        {
        }

        public override void setAttributes()
        {
            this.tenhouId = 54;
            this.name = "Aka Dora";
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
