
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      All group contains AT LEAST one terminal or one honor
    //     
    public class Chanta : Yaku
    {

        public Chanta(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Chanta";
            this.tenhouId = 23;
            this.nbHanOpen = 1;
            this.nbHanClosed = 2;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            int honors = 0;
            int terminals = 0;
            int chi = 0;
            foreach(var group in hand)
            {
                if (U.IsShuntsu(group))
                {
                    chi++;
                }
                if (U.AreAllTilesInIndices(group, C.TERMINAL_INDICES)){
                    terminals++;
                }
                if (U.AreAllTilesInIndices(group, C.HONOR_INDICES)){
                    honors++;
                }
            }
            //honroutou
            if(chi == 0)
            {
                return false;
            }

            return terminals + honors == 5 && terminals != 0 && honors != 0;
        }
    }
}
