
using kandora.bot.utils;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sanankou : Yaku
    {

        public Sanankou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Sanankou";
            this.tenhou_id = 29;
            this.han_open = 2;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            int winTile = ((int)args[0]) / 4;
            List<Meld> melds = (List<Meld>)args[1];
            bool isTsumo = (bool)args[2];
            var gc = new GroupComparer<int>();
            List<List<int>> openSets = melds.Select(x => x.tiles_34).ToList();
            var chiSets = hand.Where(x => Utils.is_chi(x) && x.Contains(winTile) && !(openSets.Contains(x, gc)));
            var ponSets = hand.Where(x => Utils.is_pon_or_kan(x));

            var closed_pons = new List<List<int>>();
            foreach(var pon in ponSets)
            {
                if(openSets.Contains(pon, gc))
                {
                    continue;
                }
                //if we do the ron on syanpon wait our pon will be consider as open
                // and it is not 789999 set
                if(pon.Contains(winTile) && !isTsumo && chiSets.Count() == 0)
                {
                    continue;
                }
                closed_pons.Add(pon);
            }
            return closed_pons.Count == 3;
        }
    }
    
}
