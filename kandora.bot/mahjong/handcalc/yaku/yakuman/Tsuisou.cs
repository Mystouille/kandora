
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Tsuisou : Yaku
    {

        public Tsuisou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Tsuisou";
            this.tenhou_id = 42;
            this.han_closed = 13;
            this.han_open = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            return indices.All(x => Constants.HONOR_INDICES.Contains(x));
        }
    }
    
}
