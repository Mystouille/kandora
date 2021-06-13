
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Pinfu : Yaku
    {

        public Pinfu(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Pinfu";
            this.tenhou_id = 7;
            this.han_open = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
