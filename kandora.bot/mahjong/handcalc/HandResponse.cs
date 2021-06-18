using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.mahjong.handcalc
{

    public class HandResponse
    {

        public Dictionary<string, int> cost;
        public int han;
        public int fu;
        public List<(int, string)> fu_details = null;
        public List<Yaku> yaku = null;
        public List<List<int>> hand;
        public string error = null;
        public object is_open_hand = false;

        public HandResponse(
            string error)
        {
            this.error = error;
        }

        public HandResponse(
            Dictionary<string, int> cost,
            int han,
            int fu,
            List<Yaku> yaku = null,
            List<List<int>> hand = null,
            string error = null,
            List<(int, string)> fu_details = null,
            bool is_open_hand = false)
        {
            this.cost = cost;
            this.han = han;
            this.fu = fu;
            this.hand = hand;
            this.error = error;
            this.is_open_hand = is_open_hand;
            if (fu_details != null)
            {
                this.fu_details = fu_details.OrderByDescending(x => x.Item1).ToList();
            }
            else
            {
                this.fu_details = null;
            }
            if (yaku != null && yaku.Count > 0)
            {
                this.yaku = yaku.OrderBy(x => x.yakuId).ToList();
            }
            else
            {
                this.yaku = null;
            }
        }

        public override string ToString()
        {
            if (this.error != null)
            {
                return this.error;
            }
            else
            {
                return $"{han} han, {fu} fu";
            }
        }
    }
}
    
