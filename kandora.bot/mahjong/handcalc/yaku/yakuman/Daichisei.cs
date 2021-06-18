
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Daichisei : Yaku
    {

        public Daichisei(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Daichisei";
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            return indices.All(x => Constants.HONOR_INDICES.Contains(x)) && hand.Count == 7;
        }
    }
    
}
