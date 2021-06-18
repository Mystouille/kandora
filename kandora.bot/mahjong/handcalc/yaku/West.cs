
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class West : Yakuhai
    {
        public West(int id) : base(id, "West")
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
            var checkPlayer = hand.Exists(x => checkKoutsu(x,Constants.WEST) && x[0] == playerWind) && playerWind == Constants.WEST;
            var checkRound = hand.Exists(x => checkKoutsu(x, Constants.WEST) && x[0] == roundWind) && roundWind == Constants.WEST;
            return checkPlayer || checkRound;
        }
    }
}
