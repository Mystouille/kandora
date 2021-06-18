
using System.Collections.Generic;
using System.Linq;
using U = kandora.bot.mahjong.Utils;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      Tsumo as dealer the first turn
    //     
    public class Daisharin : Yaku
    {
        private int count;

        public Daisharin(int yaku_id) : base(yaku_id)
        {
        }

        public override void setAttributes()
        {
            this.tenhouId = 37;
            this.name = "Daisharin";
            this.nbHanOpen = 0;
            this.nbHanClosed = 13;
            this.isYakuman = true;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            int sou = 0;
            int pin = 0;
            int man = 0;
            int honor = 0;
            if (U.IsSou(hand[0][0]))
            {
                sou++;
            }
            else if (U.IsPin(hand[0][0]))
            {
                pin++;
            }
            else if (U.IsMan(hand[0][0]))
            {
                man++;
            }
            else
            {
                honor++;
            }
            bool allowOtherSets = (bool)args[0];
            bool onlyOneSuit = sou + pin + man + honor == 1;
            if (!onlyOneSuit || honor > 0)
            {
                return false;
            }

            if(!allowOtherSets && pin == 0)
            {
                //if we are not allowing other sets than pins
                return false;
            }
            var indicesCount = new int[9];
            foreach(var set in hand)
            {
                foreach(var tile in set)
                {
                    indicesCount[U.Simplify(tile)]++;
                }
            }
            foreach(var count in indicesCount)
            {
                if(count != 2)
                {
                    return false;
                }
            }

            return true;
        }

        public void set_paarenchan_count(int count)
        {
            this.nbHanOpen = 13 * count;
            this.nbHanClosed = 13 * count;
            this.count = count;
        }

        public void rename(List<List<int>> hand)
        {
            if (U.IsSou(hand[0][0]))
            {
                this.name = "Daichikurin";
            }
            else if (U.IsPin(hand[0][0]))
            {
                this.name = "Daisharin";
            }
            else if (U.IsMan(hand[0][0]))
            {
                this.name = "Daisuurin";
            }
        }
    }
    
}
