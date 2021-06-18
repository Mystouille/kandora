
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

        public override void setAttributes()
        {
            this.name = "Sanshoku Doukou";
            this.tenhouId = 26;
            this.nbHanOpen = 2;
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var ponSets = hand.Where(x => Utils.IsKoutsuOrKantsu(x));
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            foreach (var pon in ponSets)
            {
                if (Utils.IsSou(pon[0]))
                {
                    sous.Add(pon);
                }
                if (Utils.IsMan(pon[0]))
                {
                    mans.Add(pon);
                }
                if (Utils.IsPin(pon[0]))
                {
                    pins.Add(pon);
                }
            }

            var gc = new GroupComparer<int>();
            foreach (var sou in sous)
            {

                var simpleSou = sou.Select(x => Utils.Simplify(x)).ToList();
                foreach (var man in mans)
                {
                    var simpleMan = man.Select(x => Utils.Simplify(x)).ToList();
                    foreach (var pin in pins)
                    {
                        var simplePin = pin.Select(x => Utils.Simplify(x)).ToList();
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
