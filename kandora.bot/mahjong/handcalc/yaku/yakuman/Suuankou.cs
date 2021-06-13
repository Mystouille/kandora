
using kandora.bot.utils;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Suuankou : Yaku
    {

        public Suuankou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Suuankou";
            this.tenhou_id = 41;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            int winTile = ((int)args[0]) / 4;
            bool isTsumo = (bool)args[1];
            var closed_pons = new List<List<int>>();
            foreach(var group in hand)
            {
                if (Utils.is_pon(group))
                {
                    if (group.Contains(winTile) && !isTsumo)
                    {
                        continue;
                    }
                    closed_pons.Add(group);
                }
            }
            return closed_pons.Count == 4;
        }
    }
    
}
