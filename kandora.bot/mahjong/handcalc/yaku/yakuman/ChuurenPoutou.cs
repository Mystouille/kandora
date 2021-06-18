
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class ChuurenPoutou : Yaku
    {

        public ChuurenPoutou(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Chuuren Poutou";
            this.tenhouId = 45;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            var honor = new List<List<int>>();
            foreach (var group in hand)
            {
                if (Utils.IsSou(group[0]))
                {
                    sous.Add(group);
                }
                else if (Utils.IsMan(group[0]))
                {
                    mans.Add(group);
                }
                else if (Utils.IsPin(group[0]))
                {
                    pins.Add(group);
                }
                else
                {
                    honor.Add(group);
                }
            }
            var sets = new List<List<List<int>>>
            {
                sous,mans,pins
            };
            if(sets.Where(x=>x.Count!= 0).Count() != 1)
            {
                return false;
            }

            var indices = new List<int>();
            hand.ForEach(x => x.ForEach(y => indices.Add(y)));
            var simple = indices.Select(x => Utils.Simplify(x));

            if (indices.Where(x => x == 0).Count() < 3)
            {
                return false;
            }
            if (indices.Where(x => x == 8).Count() < 3)
            {
                return false;
            }

            indices.Remove(0);
            indices.Remove(0);
            indices.Remove(8);
            indices.Remove(8);

            for (int i = 0; i<=8; i++)
            {
                if (indices.Contains(i))
                {
                    indices.Remove(i);
                }
            }

            return indices.Count == 1;
        }
    }
    
}
