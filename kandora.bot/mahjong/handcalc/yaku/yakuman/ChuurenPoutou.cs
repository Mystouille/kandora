
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

        public override void set_attributes()
        {
            this.name = "Chuuren Poutou";
            this.tenhou_id = 45;
            this.han_closed = 13;
            this.is_yakuman = true;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            var honor = new List<List<int>>();
            foreach (var group in hand)
            {
                if (Utils.is_sou(group[0]))
                {
                    sous.Add(group);
                }
                else if (Utils.is_man(group[0]))
                {
                    mans.Add(group);
                }
                else if (Utils.is_pin(group[0]))
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
            var simple = indices.Select(x => Utils.simplify(x));

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
