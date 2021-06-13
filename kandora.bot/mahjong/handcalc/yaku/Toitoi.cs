
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Toitoi : Yaku
    {

        public Toitoi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Toitoi";
            this.tenhou_id = 28;
            this.han_open = 2;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return hand.Where(x => Utils.is_pon(x)).Count() == 4;
        }
    }
    
}
