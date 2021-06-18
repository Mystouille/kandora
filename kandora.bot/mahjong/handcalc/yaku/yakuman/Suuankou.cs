
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

        public override void setAttributes()
        {
            this.name = "Suuankou";
            this.tenhouId = 41;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            int winTile = ((int)args[0]) / 4;
            bool isTsumo = (bool)args[1];
            var closed_pons = new List<List<int>>();
            foreach(var group in hand)
            {
                if (Utils.IsKoutsu(group))
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
