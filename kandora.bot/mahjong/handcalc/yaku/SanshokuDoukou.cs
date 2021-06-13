
using kandora.bot.utils;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class SanshokuDoukou : Yaku
    {

        public SanshokuDoukou(int id) : base(id)
        {
        }

        public override void set_attributes()
        {
            this.name = "Sanshoku Doukou";
            this.tenhou_id = 26;
            this.han_open = 2;
            this.han_closed = 2;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var ponSets = hand.Where(x => Utils.is_pon_or_kan(x));
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            foreach (var pon in ponSets)
            {
                if (Utils.is_sou(pon[0]))
                {
                    sous.Add(pon);
                }
                if (Utils.is_man(pon[0]))
                {
                    mans.Add(pon);
                }
                if (Utils.is_pin(pon[0]))
                {
                    pins.Add(pon);
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
