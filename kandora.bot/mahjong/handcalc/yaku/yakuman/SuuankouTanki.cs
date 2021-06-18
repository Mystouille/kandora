
using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      
    //     
    public class SuuankouTanki : Yaku
    {

        public SuuankouTanki(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Suuankou Tanki";
            this.tenhouId = 40;
            this.nbHanClosed = 26;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            return true;
        }
    }
    
}
