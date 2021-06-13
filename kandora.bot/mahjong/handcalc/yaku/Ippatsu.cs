
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Ippatsu : Yaku
    {

        public Ippatsu(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Ippatsu";
            this.tenhou_id = 2;
            this.han_closed = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
