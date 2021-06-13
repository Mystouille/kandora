
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Ryuuiisou : Yaku
    {

        public Ryuuiisou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Ryuuiisou";
            this.tenhou_id = 43;
            this.han_open = 13;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var green = new int[] { 19, 20, 21, 23, 25, Constants.HATSU };
            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            return indices.All(x => green.Contains(x));
        }
    }
    
}
