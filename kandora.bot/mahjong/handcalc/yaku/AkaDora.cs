
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku
{

    // 
    //     Red five
    //     
    public class AkaDora : Yaku
    {

        public AkaDora(object yaku_id = null)
        {
        }

        public override void set_attributes()
        {
            this.tenhou_id = 54;
            this.name = "Aka Dora";
            this.han_open = 1;
            this.han_closed = 1;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }

        public override string ToString()
        {
            return $"{this.name} {this.han_closed}";
        }
    }
    
}
