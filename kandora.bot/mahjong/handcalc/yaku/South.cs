
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class South : Yakuhai
    {

        public South(int id) : base(id, "South")
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
            var checkPlayer = hand.Exists(x => checkKoutsu(x,Constants.SOUTH) && x[0] == playerWind) && playerWind == Constants.SOUTH;
            var checkRound = hand.Exists(x => checkKoutsu(x, Constants.SOUTH) && x[0] == roundWind) && roundWind == Constants.SOUTH;
            return checkPlayer || checkRound;
        }
    }
}
