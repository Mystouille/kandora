using System.Collections.Generic;

namespace kandora.bot.mahjong.handcalc
{
    public abstract class Yaku
    {

        public int yaku_id;
        public int tenhou_id;
        public virtual string name { get; set; }
        public int han_open;
        public int han_closed;
        public bool is_yakuman;

        public Yaku(int yaku_id)
        {
            this.tenhou_id = -1;
            this.yaku_id = yaku_id;
            this.han_closed = 0;
            this.han_open = 0;
            this.set_attributes();
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
        public abstract bool is_condition_met(List<List<int>> hand, params object[] args);

        // 
        //         Set id, name, han related to the yaku
        //         
        public abstract void set_attributes();

    }
    
}
