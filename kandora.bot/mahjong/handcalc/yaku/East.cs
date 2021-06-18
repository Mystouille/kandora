
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class East : Yakuhai
    {

        public East(int id) : base(id, "East")
        {
        }

        public override void setAttributes()
        {
            base.setAttributes();
            this.tenhouId = 10;
        }
        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var playerWind = (int)args[0];
            var roundWind = (int)args[1];
            var checkPlayer = hand.Exists(x => checkKoutsu(x,Constants.EAST) && x[0] == playerWind) && playerWind == Constants.EAST;
            var checkRound = hand.Exists(x => checkKoutsu(x, Constants.EAST) && x[0] == roundWind) && roundWind == Constants.EAST;
            return checkPlayer || checkRound;
        }
    }
}
