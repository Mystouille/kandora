
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //    koustu of red dragon  
    //     
    public class Haku : Yakuhai
    {

        public Haku(int id) : base(id, "Haku")
        {
        }

        public override void setAttributes()
        {
            base.setAttributes();
            this.tenhouId = 18;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return hand.Exists(x => checkKoutsu(x, Constants.HAKU));
        }
    }
    
}
