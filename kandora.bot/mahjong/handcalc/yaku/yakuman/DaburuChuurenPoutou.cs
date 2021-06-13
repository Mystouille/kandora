
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class DaburuChuurenPoutou : Yaku
    {

        public DaburuChuurenPoutou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Daburu Chuuren Poutou";
            this.tenhou_id = 46;
            this.han_closed = 26;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
