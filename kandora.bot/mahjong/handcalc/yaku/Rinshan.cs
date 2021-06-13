
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Rinshan : Yaku
    {

        public Rinshan(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Rinshan Kaihou";
            this.tenhou_id = 4;
            this.han_open = 1;
            this.han_closed = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
