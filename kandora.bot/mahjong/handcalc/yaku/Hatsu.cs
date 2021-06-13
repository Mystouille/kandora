
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

        public override void set_attributes()
        {
            base.set_attributes();
            this.tenhou_id = 19;
        }

        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            return hand.Exists(x => checkKoutsu(x, Constants.HATSU));
        }
    }
    
}
