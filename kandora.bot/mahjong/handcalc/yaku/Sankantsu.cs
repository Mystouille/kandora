
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Sankantsu : Yaku
    {

        public Sankantsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Sankantsu";
            this.tenhouId = 27;
            this.nbHanOpen = 2;
            this.nbHanClosed = 2;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            List<Meld> melds = (List<Meld>)args[0];
            var kans = melds.Where(x => x.type == Meld.KAN || x.type == Meld.SHOUMINKAN);
            return kans.Count() == 3;
        }
    }
    
}
