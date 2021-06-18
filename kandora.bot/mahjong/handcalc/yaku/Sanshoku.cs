
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

        public override void setAttributes()
        {
            this.name = "Sanshoku";
            this.tenhouId = 25;
            this.nbHanOpen = 1;
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var chiSets = hand.Where(x => Utils.IsShuntsu(x));
            var sous = new List<List<int>>();
            var pins = new List<List<int>>();
            var mans = new List<List<int>>();
            foreach (var chi in chiSets)
            {
                if (Utils.IsSou(chi[0]))
                {
                    sous.Add(chi);
                }
                if (Utils.IsMan(chi[0]))
                {
                    mans.Add(chi);
                }
                if (Utils.IsPin(chi[0]))
                {
                    pins.Add(chi);
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
