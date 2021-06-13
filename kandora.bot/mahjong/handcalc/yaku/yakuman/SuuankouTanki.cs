
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class SuuankouTanki : Yaku
    {

        public SuuankouTanki(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Suuankou Tanki";
            this.tenhou_id = 40;
            this.han_closed = 26;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
