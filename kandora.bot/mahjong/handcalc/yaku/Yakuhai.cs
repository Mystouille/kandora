
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class Yakuhai : Yaku
    {
        protected string windName;
        public Yakuhai(int id, string windName) : base(id)
        {
            this.windName = windName;
        }

        public override string name { get
            {
                return $"Yakuhai ({windName})";
            }
        }
        public override void set_attributes()
        {
            this.han_open = 1;
            this.han_closed = 1;
            this.is_yakuman = false;
        }

        public override string ToString()
        {
            return $"{this.name} {this.han_closed}";
        }
        public override bool is_condition_met(List<List<int>> hand, params object[] args)
        {
            var playerWind = (int)args[0];
            var roundWind = (int)args[1];
            var checkPlayer = hand.Exists(x => (Utils.is_pon_or_kan(x) && x[0] == Constants.EAST) && x[0] == playerWind) && playerWind == Constants.EAST;
            var checkRound = hand.Exists(x => (Utils.is_pon_or_kan(x) && x[0] == Constants.EAST) && x[0] == roundWind) && roundWind == Constants.EAST;
            return checkPlayer || checkRound;
        }

        protected bool checkKoutsu(List<int> group, int tile)
        {
            return (Utils.is_pon_or_kan(group) && group[0] == tile);
        }
    }
    
}
