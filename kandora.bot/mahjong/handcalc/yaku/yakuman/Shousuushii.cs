
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Shousuushii : Yaku
    {

        public Shousuushii(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Shousuushii";
            this.tenhouId = 50;
            this.nbHanOpen = 13;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var pons = hand.Where(x => Utils.IsKoutsuOrKantsu(x));
            if (pons.Count() < 3)
            {
                return false;
            }
            var nbWindPon = 0;
            var nbWindPair = 0;
            var winds = new int[] { Constants.EAST, Constants.WEST, Constants.SOUTH, Constants.NORTH };
            foreach(var group in hand)
            {
                if (Utils.IsKoutsuOrKantsu(group) && winds.Contains(group[0]))
                {
                    nbWindPon++;
                }
                if (Utils.IsPair(group) && winds.Contains(group[0]))
                {
                    nbWindPair++;
                }
            }
            return nbWindPon == 3 && nbWindPair == 1;
        }
    }
    
}
