
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo on last draw
    //     
    public class Haitei : Yaku
    {

        public Haitei(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Haitei Raoyue";
            this.tenhou_id = 5;
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
