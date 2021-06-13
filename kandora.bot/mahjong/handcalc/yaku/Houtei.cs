
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo on last draw
    //     
    public class Houtei : Yaku
    {

        public Houtei(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Haitei Raoyui";
            this.tenhou_id = 6;
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
