
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Daisuushii : Yaku
    {

        public Daisuushii(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Daisuushii";
            this.tenhouId = 49;
            this.nbHanOpen = 26;
            this.nbHanClosed = 26;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var pons = hand.Where(x => Utils.IsKoutsuOrKantsu(x));
            if (pons.Count() != 4)
            {
                return false;
            }
            var nbWindPon = 0;
            var winds = new int[] { Constants.EAST, Constants.WEST, Constants.SOUTH, Constants.NORTH };
            foreach(var group in hand)
            {
                if (Utils.IsKoutsuOrKantsu(group) && winds.Contains(group[0]))
                {
                    nbWindPon++;
                }
            }
            return nbWindPon == 4;
        }
    }
    
}
