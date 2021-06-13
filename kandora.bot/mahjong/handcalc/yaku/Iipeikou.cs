
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;
using System.Linq;
using kandora.bot.utils;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      All group contains AT LEAST one terminal or one honor
    //     
    public class Iipeikou : Yaku
    {

        public Iipeikou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Iipeikou";
            this.tenhou_id = 9;
            this.han_closed = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var chis = hand.Where(x => Utils.is_chi(x));
            return chis.Distinct(new GroupComparer<int>()).Count() != chis.Count();
        }
    }
}
