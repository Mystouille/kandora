
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      The hand contains tiles only from a single suit and honors
    //     
    public class Honroutou : Yaku
    {

        public Honroutou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Honroutou";
            this.tenhouId = 31;
            this.nbHanOpen = 2;
            this.nbHanClosed = 2;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            foreach(var group in hand)
            {
                if(!group.All(x=>Constants.HONOR_INDICES.Contains(x) || Constants.TERMINAL_INDICES.Contains(x)))
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
