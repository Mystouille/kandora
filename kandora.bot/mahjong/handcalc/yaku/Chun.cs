
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //    koustu of red dragon  
    //     
    public class Chun : Yakuhai
    {

        public Chun(int id) : base(id,"Chun")
        {
        }

        public override void setAttributes()
        {
            base.setAttributes();
            this.tenhouId = 20;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return hand.Exists(x => checkKoutsu(x, Constants.CHUN));
        }
    }
    
}
