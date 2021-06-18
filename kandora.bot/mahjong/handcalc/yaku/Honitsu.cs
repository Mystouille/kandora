
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;
using System.Linq;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      The hand contains tiles only from a single suit and honors
    //     
    public class Honitsu : Yaku
    {

        public Honitsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Honitsu";
            this.tenhouId = 34;
            this.nbHanOpen = 2;
            this.nbHanClosed = 3;
            this.isYakuman = false;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            int honor = 0;
            int sou = 0;
            int pin = 0;
            int man = 0;
            foreach (var group in hand)
            {
                if (C.HONOR_INDICES.Contains(group[0])){
                    honor++;
                }

                if (U.IsSou(group[0]))
                {
                    sou++;
                }
                else if (U.IsMan(group[0]))
                {
                    man++;
                }
                else if (U.IsPin(group[0]))
                {
                    pin++;
                }
            }

            return honor >0
                && ((sou > 0 && pin + man == 0)
                    || (man > 0 && pin + sou == 0)
                    || (pin > 0 && sou + man == 0)
                );
        }
    }
    
}
