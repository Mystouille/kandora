
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Suukantsu : Yaku
    {

        public Suukantsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Suukantsu";
            this.tenhouId = 51;
            this.nbHanOpen = 13;
            this.nbHanClosed = 13;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            List<Meld> melds = (List<Meld>)args[0];
            var kans = melds.Where(x => x.type == Meld.KAN || x.type == Meld.SHOUMINKAN);
            return kans.Count() == 4;
        }
    }
    
}
