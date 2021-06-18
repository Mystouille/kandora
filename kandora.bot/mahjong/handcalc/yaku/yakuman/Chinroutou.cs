
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      The hand contains tiles only from a single suit and honors
    //     
    public class Chinroutou : Yaku
    {

        public Chinroutou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Chinroutou";
            this.tenhouId = 44;
            this.nbHanOpen = 13;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            foreach(var group in hand)
            {
                if(!group.All(x=>Constants.TERMINAL_INDICES.Contains(x)))
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
