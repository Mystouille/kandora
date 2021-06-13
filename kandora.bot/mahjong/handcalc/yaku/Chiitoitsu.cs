
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Hand is composed of 7 different pairs
    //     
    public class Chiitoitsu : Yaku
    {

        public Chiitoitsu(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Chiitoitsu";
            this.tenhou_id = 22;
            this.han_open = 0;
            this.han_closed = 2;
            this.is_yakuman = false;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {

            var indices = new List<int>();
            foreach(var group in hand)
            {
                indices.Add(group[0]);
            }
            //Pairs must be all different
            return hand.Count == 7 && indices.Distinct().Count() == hand.Count;
        }
    }
    
}
