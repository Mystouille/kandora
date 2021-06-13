
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class North : Yakuhai
    {

        public North(int id) : base(id, "North")
        {
        }

        public override void set_attributes()
        {
            base.set_attributes();
            this.tenhou_id = 10;
        }
        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var playerWind = (int)args[0];
            var roundWind = (int)args[1];
            var checkPlayer = hand.Exists(x => checkKoutsu(x,Constants.NORTH) && x[0] == playerWind) && playerWind == Constants.NORTH;
            var checkRound = hand.Exists(x => checkKoutsu(x, Constants.NORTH) && x[0] == roundWind) && roundWind == Constants.NORTH;
            return checkPlayer || checkRound;
        }
    }
}
