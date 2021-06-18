
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Shousangen : Yaku
    {

        public Shousangen(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Shousangen";
            this.tenhouId = 30;
            this.nbHanOpen = 2;
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var dragons = new int[] { Constants.CHUN, Constants.HAKU, Constants.HATSU };
            var count = 0;
            foreach(var group in hand)
            {
                if((Utils.IsPair(group) || Utils.IsKoutsuOrKantsu(group)) && dragons.Contains(group[0]))
                {
                    count++;
                }
            }
            return count == 3;
        }
    }
    
}
