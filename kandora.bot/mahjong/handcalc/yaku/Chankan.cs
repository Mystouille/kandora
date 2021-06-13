
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Robbing a kan
    //     
    public class Chankan : Yaku
    {

        public Chankan(int yaku_id): base(yaku_id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Chankan";
            this.tenhou_id = 3;
            this.han_open = 1;
            this.han_closed = 1;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
