
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Hand is composed of 7 different pairs
    //     
    public class Chiitoitsu : Yaku
    {

        public Chiitoitsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Chiitoitsu";
            this.tenhouId = 22;
            this.nbHanOpen = 0;
            this.nbHanClosed = 2;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {

            var indices = new List<int>();
            foreach(var group in hand)
            {
                indices.Add(group[0]);
            }
            //Pairs must be all different
            return hand.Count == 7 && indices.Distinct().Count() == hand.Count;
        }
    }
    
}
