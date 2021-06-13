
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class NagashiMangan : Yaku
    {

        public NagashiMangan(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Nagashi Mangan";
            this.han_open = 5;
            this.han_closed = 5;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
