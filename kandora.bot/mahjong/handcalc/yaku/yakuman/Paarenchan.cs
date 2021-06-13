
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Paarenchan : Yaku
    {
        private int count;

        public Paarenchan(int yaku_id) : base(yaku_id)
        {
        }

        public override void set_attributes()
        {
            this.tenhou_id = 37;
            this.name = "Tenhou";
            this.han_open = 0;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }

        public void set_paarenchan_count(int count)
        {
            this.han_open = 13 * count;
            this.han_closed = 13 * count;
            this.count = count;
        }
    }
    
}
