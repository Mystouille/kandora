
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

        public override void set_attributes()
        {
            this.name = "Honroutou";
            this.tenhou_id = 31;
            this.han_open = 2;
            this.han_closed = 2;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
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
