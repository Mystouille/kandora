
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Renhou : Yaku
    {

        public Renhou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Renhou";
            this.tenhou_id = 36;
            this.han_closed = 5;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
