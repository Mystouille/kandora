
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

        public override void set_attributes()
        {
            this.name = "Chanta";
            this.tenhou_id = 23;
            this.han_open = 1;
            this.han_closed = 2;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            int honors = 0;
            int terminals = 0;
            int chi = 0;
            foreach(var group in hand)
            {
                if (U.is_chi(group))
                {
                    chi++;
                }
                if (U.are_tiles_in_indices(group, C.TERMINAL_INDICES)){
                    terminals++;
                }
                if (U.are_tiles_in_indices(group, C.HONOR_INDICES)){
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
