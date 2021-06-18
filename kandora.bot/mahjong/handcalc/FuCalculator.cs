using System;
using System.Collections.Generic;
using System.Linq;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong.handcalc
{

    public class FuCalculator
    {

        public const string BASE = "base";
        public const string PENCHAN = "penchan";
        public const string KANCHAN = "kanchan";
        public const string VALUED_PAIR = "valued_pair";
        public const string DOUBLE_VALUED_PAIR = "double_valued_pair";
        public const string PAIR_WAIT = "pair_wait";
        public const string TSUMO = "tsumo";
        public const string HAND_WITHOUT_FU = "hand_without_fu";
        public const string CLOSED_PON = "closed_pon";
        public const string OPEN_PON = "open_pon";
        public const string CLOSED_TERMINAL_PON = "closed_terminal_pon";
        public const string OPEN_TERMINAL_PON = "open_terminal_pon";
        public const string CLOSED_KAN = "closed_kan";
        public const string OPEN_KAN = "open_kan";
        public const string CLOSED_TERMINAL_KAN = "closed_terminal_kan";
        public const string OPEN_TERMINAL_KAN = "open_terminal_kan";
   
        /// <summary>
        /// Calculate the number of fu of a winning hand
        /// </summary>
        /// <param name="hand">The hand in 136 format</param>
        /// <param name="winTile">Winning tile, 136 format</param>
        /// <param name="winGroup">The group where the wile tile is, 34 format</param>
        /// <param name="config">The hand config</param>
        /// <param name="valuedTiles">Dragons, player wind, round wind</param>
        /// <param name="melds">Opened sets</param>
        /// <returns>A list of the fu details and the fu value rounded up to the 10s</returns>
        public static (List<(int, string)>, int) CalculateFu(
            List<List<int>> hand,
            int winTile,
            List<int> winGroup,
            HandConfig config,
            List<int> valuedTiles = null,
            List<Meld> melds = null)
        {
            var winTile34 = winTile / 4;
            if (valuedTiles == null)
            {
                valuedTiles = new List<int>();
            }
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            var fuDetails = new List<(int, string)>();
            if (hand.Count == 7)
            {
                return (new List<(int,string)>() { (25, BASE) }, 25);
            }
            var pair = (from x in hand
                        where U.IsPair(x)
                        select x).ToList()[0];
            var koutsuList = (from x in hand
                            where U.IsKoutsuOrKantsu(x)
                            select x).ToList();
            var copiedOpenMelds = (from x in melds
                                        where x.type == Meld.CHI
                                        select x.Tiles34).ToList();
            var closedShoutsuSets = new List<List<int>>();
            foreach (var x in hand)
            {
                if (!copiedOpenMelds.Contains(x))
                {
                    closedShoutsuSets.Add(x);
                }
                else
                {
                    copiedOpenMelds.Remove(x);
                }
            }
            var isOpenHand = (from x in melds
                                    select x.opened).Any();
            if (closedShoutsuSets.Contains(winGroup))
            {
                var tileIdx = U.Simplify(winTile34);
                // penchan
                if (U.ContainsTerminal(winGroup))
                {
                    // 1-2-... wait
                    if (tileIdx == 2 && winGroup.FindIndex(x => x == winTile34) == 2)
                    {
                        fuDetails.Add((2,PENCHAN));
                    }
                    else if (tileIdx == 6 && winGroup.FindIndex(x => x == winTile34) == 0)
                    {
                        // 8-9-... wait
                        fuDetails.Add((2, PENCHAN));
                    }
                }
                // kanchan waiting 5-...-7
                if (winGroup.FindIndex(x => x == winTile34) == 1)
                {
                    fuDetails.Add((2, KANCHAN));
                }
            }
            // valued pair
            var valuedPairsCount = valuedTiles.Count(x => x==pair[0]);
            if (valuedPairsCount == 1)
            {
                fuDetails.Add((2, VALUED_PAIR));
            }
            // east-east pair when you are on east gave double fu
            if (valuedPairsCount == 2)
            {
                fuDetails.Add((4, DOUBLE_VALUED_PAIR));
            }
            // pair wait
            if (U.IsPair(winGroup))
            {
                fuDetails.Add((2, PAIR_WAIT));
            }
            foreach (var koutsu in koutsuList)
            {
                var openMeld = (from x in melds
                                    where koutsu == x.Tiles34
                                    select x).ToList().FirstOrDefault();
                var setWasOpen = openMeld != null && openMeld.opened;
                var isKantsu = openMeld != null && (openMeld.type == Meld.KAN || openMeld.type == Meld.SHOUMINKAN);
                var isHonor = (C.TERMINAL_INDICES.Concat(C.HONOR_INDICES)).Contains(koutsu[0]);
                // we win by ron on the third pon tile, our pon will be count as open
                if (!config.isTsumo && koutsu == winGroup)
                {
                    setWasOpen = true;
                }
                if (isHonor)
                {
                    if (isKantsu)
                    {
                        if (setWasOpen)
                        {
                            fuDetails.Add((16, OPEN_TERMINAL_KAN));
                        }
                        else
                        {
                            fuDetails.Add((32, CLOSED_TERMINAL_KAN));
                        }
                    }
                    else if (setWasOpen)
                    {
                        fuDetails.Add((4, OPEN_TERMINAL_PON));
                    }
                    else
                    {
                        fuDetails.Add((8, CLOSED_TERMINAL_PON));
                    }
                }
                else if (isKantsu)
                {
                    if (setWasOpen)
                    {
                        fuDetails.Add((8, OPEN_KAN));
                    }
                    else
                    {
                        fuDetails.Add((16, CLOSED_KAN));
                    }
                }
                else if (setWasOpen)
                {
                    fuDetails.Add((2, OPEN_PON));
                }
                else
                {
                    fuDetails.Add((4, CLOSED_PON));
                }
            }
            var addTsumoFu = fuDetails.Count > 0 || config.options.fuForPinfuTsumo;
            if (config.isTsumo && addTsumoFu)
            {
                // 2 additional fu for tsumo (but not for pinfu)
                fuDetails.Add((2, TSUMO));
            }
            if (isOpenHand && fuDetails.Count == 0 && config.options.fuForOpenPinfu)
            {
                // there is no 1-20 hands, so we have to add additional fu
                fuDetails.Add((2, HAND_WITHOUT_FU));
            }
            if (isOpenHand || config.isTsumo)
            {
                fuDetails.Add((20, BASE));
            }
            else
            {
                fuDetails.Add((30, BASE));
            }
            return (fuDetails, RoundFu(fuDetails));
        }

        private static int RoundFu(List<(int,string)> fuDetails)
        {
            // 22 -> 30 and etc.
            var fu = (from x in fuDetails
                        select x.Item1).ToList().Sum();
            return (fu + 9) / 10 * 10;
        }
    }
    
}
