
using kandora.bot.utils;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sanshoku : Yaku
    {

        public Sanshoku(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Sanshoku";
            this.tenhou_id = 25;
            this.han_open = 1;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var chiSets = hand.Where(x => Utils.is_chi(x));
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            foreach (var chi in chiSets)
            {
                if (Utils.is_sou(chi[0]))
                {
                    sous.Add(chi);
                }
                if (Utils.is_man(chi[0]))
                {
                    mans.Add(chi);
                }
                if (Utils.is_pin(chi[0]))
                {
                    pins.Add(chi);
                }
            }

            var gc = new GroupComparer<int>();
            foreach (var sou in sous)
            {

                var simpleSou = sou.Select(x => Utils.simplify(x)).ToList();
                foreach (var man in mans)
                {
                    var simpleMan = man.Select(x => Utils.simplify(x)).ToList();
                    foreach (var pin in pins)
                    {
                        var simplePin = pin.Select(x => Utils.simplify(x)).ToList();
                        if (simpleMan.SequenceEqual(simplePin) && simpleMan.SequenceEqual(simpleSou))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
    
}
