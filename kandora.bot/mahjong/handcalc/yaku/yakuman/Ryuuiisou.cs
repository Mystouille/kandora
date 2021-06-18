
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Ryuuiisou : Yaku
    {

        public Ryuuiisou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Ryuuiisou";
            this.tenhouId = 43;
            this.nbHanOpen = 13;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var green = new int[] { 19, 20, 21, 23, 25, Constants.HATSU };
            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            return indices.All(x => green.Contains(x));
        }
    }
    
}
