
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Tsuisou : Yaku
    {

        public Tsuisou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Tsuisou";
            this.tenhouId = 42;
            this.nbHanClosed = 13;
            this.nbHanOpen = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            return indices.All(x => Constants.HONOR_INDICES.Contains(x));
        }
    }
    
}
