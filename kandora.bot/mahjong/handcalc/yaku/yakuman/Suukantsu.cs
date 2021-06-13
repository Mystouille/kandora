
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Suukantsu : Yaku
    {

        public Suukantsu(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Suukantsu";
            this.tenhou_id = 51;
            this.han_open = 13;
            this.han_closed = 13;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            List<Meld> melds = (List<Meld>)args[0];
            var kans = melds.Where(x => x.type == Meld.KAN || x.type == Meld.SHOUMINKAN);
            return kans.Count() == 4;
        }
    }
    
}
