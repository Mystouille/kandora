
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Kokushi : Yaku
    {

        public Kokushi(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Kokushi Musou";
            this.tenhou_id = 47;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var tiles_34 = (int[])args[0];
            return tiles_34[0]
                * tiles_34[8]
                * tiles_34[9]
                * tiles_34[17]
                * tiles_34[18]
                * tiles_34[26]
                * tiles_34[27]
                * tiles_34[28]
                * tiles_34[29]
                * tiles_34[30]
                * tiles_34[31]
                * tiles_34[32]
                * tiles_34[33]
                == 2;
        }
    }
    
}
