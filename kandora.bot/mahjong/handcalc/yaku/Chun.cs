
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

        public override void set_attributes()
        {
            base.set_attributes();
            this.tenhou_id = 20;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return hand.Exists(x => checkKoutsu(x, Constants.CHUN));
        }
    }
    
}
