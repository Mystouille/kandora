
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Tanyao : Yaku
    {

        public Tanyao(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Tanyao";
            this.tenhouId = 8;
            this.nbHanOpen = 1;
            this.nbHanClosed = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var indices = new List<int>();
            hand.ForEach(group=> group.ForEach(x => indices.Add(x)));
            return indices.Intersect(Constants.TERMINAL_INDICES).Count() + indices.Intersect(Constants.HONOR_INDICES).Count() == 0;
        }
    }
    
}
