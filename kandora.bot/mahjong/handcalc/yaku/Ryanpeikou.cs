
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
    public class Ryanpeikou : Yaku
    {

        public Ryanpeikou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Ryanpeikou";
            this.tenhouId = 32;
            this.nbHanClosed = 3;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var count = 0;
            var chis = hand.Where(x => Utils.IsShuntsu(x));
            var nbIdenticalChis = new List<int>();
            foreach (var chi1 in chis)
            {
                count = 0;
                foreach (var chi2 in chis)
                {
                    if (chi1.SequenceEqual(chi2))
                    {
                        count++;
                    }
                }
                nbIdenticalChis.Add(count);
            }
            return nbIdenticalChis.Where(x => x >= 2).Count() == 4;
        }
    }
}
