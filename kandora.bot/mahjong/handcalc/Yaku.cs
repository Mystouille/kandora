using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc
{
    public abstract class Yaku
    {

        public int yakuId;
        public int tenhouId;
        public virtual string name { get; set; }
        public int nbHanOpen;
        public int nbHanClosed;
        public bool isYakuman;

        public Yaku(int yaku_id)
        {
            this.tenhouId = -1;
            this.yakuId = yaku_id;
            this.nbHanClosed = 0;
            this.nbHanOpen = 0;
            this.setAttributes();
        }

        public override string ToString()
        {
            return this.name;
        }

        // 
        //         Is this yaku exists in the hand?
        //         :param: hand
        //         :param: args: some yaku requires additional attributes
        //         :return: boolean
        //         
        public abstract bool isConditionMet(List<List<int>> hand, params object[] args);

        // 
        //         Set id, name, han related to the yaku
        //         
        public abstract void setAttributes();

    }
    
}
