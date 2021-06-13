
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Tanyao : Yaku
    {

        public Tanyao(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Tanyao";
            this.tenhou_id = 8;
            this.han_open = 1;
            this.han_closed = 1;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var indices = new List<int>();
            hand.ForEach(group=> group.ForEach(x => indices.Add(x)));
            return indices.Intersect(Constants.TERMINAL_INDICES).Count() + indices.Intersect(Constants.HONOR_INDICES).Count() == 0;
        }
    }
    
}
