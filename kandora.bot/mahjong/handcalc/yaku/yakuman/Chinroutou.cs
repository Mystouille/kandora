
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

        public override void set_attributes()
        {
            this.name = "Chinroutou";
            this.tenhou_id = 44;
            this.han_open = 13;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
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
