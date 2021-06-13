
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sankantsu : Yaku
    {

        public Sankantsu(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Sankantsu";
            this.tenhou_id = 27;
            this.han_open = 2;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            List<Meld> melds = (List<Meld>)args[0];
            var kans = melds.Where(x => x.type == Meld.KAN || x.type == Meld.SHOUMINKAN);
            return kans.Count() == 3;
        }
    }
    
}
