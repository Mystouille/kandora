
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Daisangen : Yaku
    {

        public Daisangen(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Daisangen";
            this.tenhouId = 39;
            this.nbHanOpen = 13;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var dragons = new int[] { Constants.CHUN, Constants.HAKU, Constants.HATSU };
            var count = 0;
            foreach(var group in hand)
            {
                if(Utils.IsKoutsuOrKantsu(group) && dragons.Contains(group[0]))
                {
                    count++;
                }
            }
            return count == 3;
        }
    }
    
}
