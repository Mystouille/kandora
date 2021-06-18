
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //    koustu of red dragon  
    //     
    public class Hatsu : Yakuhai
    {

        public Hatsu(int id) : base(id, "Hatsu")
        {
        }

        public override void setAttributes()
        {
            base.setAttributes();
            this.tenhouId = 19;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return hand.Exists(x => checkKoutsu(x, Constants.HATSU));
        }
    }
    
}
