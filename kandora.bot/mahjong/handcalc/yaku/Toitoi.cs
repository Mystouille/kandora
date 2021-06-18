
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Toitoi : Yaku
    {

        public Toitoi(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Toitoi";
            this.tenhouId = 28;
            this.nbHanOpen = 2;
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return hand.Where(x => Utils.IsKoutsu(x)).Count() == 4;
        }
    }
    
}
