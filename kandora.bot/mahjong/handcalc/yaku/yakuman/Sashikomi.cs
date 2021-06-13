
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sashikomi : Yaku
    {

        public Sashikomi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Sashikomi";
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
