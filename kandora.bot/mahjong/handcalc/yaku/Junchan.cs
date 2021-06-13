
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

        public override void set_attributes()
        {
            this.name = "Junchan";
            this.tenhou_id = 33;
            this.han_open = 2;
            this.han_closed = 3;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            int terminals = 0;
            int chi = 0;
            foreach(var group in hand)
            {
                if (U.is_chi(group))
                {
                    chi++;
                }
                if (U.are_tiles_in_indices(group, C.TERMINAL_INDICES)) {
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
