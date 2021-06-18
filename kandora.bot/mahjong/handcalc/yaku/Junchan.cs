
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      All group contains AT LEAST one terminal or one honor
    //     
    public class Junchan : Yaku
    {

        public Junchan(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Junchan";
            this.tenhouId = 33;
            this.nbHanOpen = 2;
            this.nbHanClosed = 3;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            int terminals = 0;
            int chi = 0;
            foreach(var group in hand)
            {
                if (U.IsShuntsu(group))
                {
                    chi++;
                }
                if (U.AreAllTilesInIndices(group, C.TERMINAL_INDICES)) {
                    terminals++;
                }
            }
            //honroutou
            if(chi == 0)
            {
                return false;
            }

            return terminals == 5;
        }
    }    
}
