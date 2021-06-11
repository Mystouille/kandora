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

        // 
        //         Calculate hand fu with explanations
        //         :param hand: 
        //         :param win_tile: 136 tile format
        //         :param win_group: one set where win tile exists
        //         :param config: HandConfig object
        //         :param valued_tiles: dragons, player wind, round wind
        //         :param melds: opened sets
        //         :return:
        //         
        public static (List<(int, string)>, int) calculate_fu(
            List<List<int>> hand,
            int win_tile,
            List<int> win_group,
            HandConfig config,
            List<int> valued_tiles = null,
            List<Meld> melds = null)
        {
            var win_tile_34 = win_tile / 4;
            if (valued_tiles == null)
            {
                valued_tiles = new List<int>();
            }
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            var fu_details = new List<(int, string)>();
            if (hand.Count == 7)
            {
                return (new List<(int,string)>() { (25, BASE) }, 25);
            }
            var pair = (from x in hand
                        where U.is_pair(x)
                        select x).ToList()[0];
            var pon_sets = (from x in hand
                            where U.is_pon_or_kan(x)
                            select x).ToList();
            var copied_opened_melds = (from x in melds
                                        where x.type == Meld.CHI
                                        select x.tiles_34).ToList();
            var closed_chi_sets = new List<List<int>>();
            foreach (var x in hand)
            {
                if (!copied_opened_melds.Contains(x))
                {
                    closed_chi_sets.Add(x);
                }
                else
                {
                    copied_opened_melds.Remove(x);
                }
            }
            var is_open_hand = (from x in melds
                                    select x.opened).Any();
            if (closed_chi_sets.Contains(win_group))
            {
                var tile_index = U.simplify(win_tile_34);
                // penchan
                if (U.contains_terminals(win_group))
                {
                    // 1-2-... wait
                    if (tile_index == 2 && win_group.FindIndex(x => x == win_tile_34) == 2)
                    {
                        fu_details.Add((2,PENCHAN));
                    }
                    else if (tile_index == 6 && win_group.FindIndex(x => x == win_tile_34) == 0)
                    {
                        // 8-9-... wait
                        fu_details.Add((2, PENCHAN));
                    }
                }
                // kanchan waiting 5-...-7
                if (win_group.FindIndex(x => x == win_tile_34) == 1)
                {
                    fu_details.Add((2, KANCHAN));
                }
            }
            // valued pair
            var count_of_valued_pairs = valued_tiles.Count(x => x==pair[0]);
            if (count_of_valued_pairs == 1)
            {
                fu_details.Add((2, VALUED_PAIR));
            }
            // east-east pair when you are on east gave double fu
            if (count_of_valued_pairs == 2)
            {
                fu_details.Add((4, DOUBLE_VALUED_PAIR));
            }
            // pair wait
            if (U.is_pair(win_group))
            {
                fu_details.Add((2, PAIR_WAIT));
            }
            foreach (var set_item in pon_sets)
            {
                var open_meld = (from x in melds
                                    where set_item == x.tiles_34
                                    select x).ToList().FirstOrDefault();
                var set_was_open = open_meld != null && open_meld.opened;
                var is_kan_set = open_meld != null && (open_meld.type == Meld.KAN || open_meld.type == Meld.SHOUMINKAN);
                var is_honor = (C.TERMINAL_INDICES.Concat(C.HONOR_INDICES)).Contains(set_item[0]);
                // we win by ron on the third pon tile, our pon will be count as open
                if (!config.is_tsumo && set_item == win_group)
                {
                    set_was_open = true;
                }
                if (is_honor)
                {
                    if (is_kan_set)
                    {
                        if (set_was_open)
                        {
                            fu_details.Add((16, OPEN_TERMINAL_KAN));
                        }
                        else
                        {
                            fu_details.Add((32, CLOSED_TERMINAL_KAN));
                        }
                    }
                    else if (set_was_open)
                    {
                        fu_details.Add((4, OPEN_TERMINAL_PON));
                    }
                    else
                    {
                        fu_details.Add((8, CLOSED_TERMINAL_PON));
                    }
                }
                else if (is_kan_set)
                {
                    if (set_was_open)
                    {
                        fu_details.Add((8, OPEN_KAN));
                    }
                    else
                    {
                        fu_details.Add((16, CLOSED_KAN));
                    }
                }
                else if (set_was_open)
                {
                    fu_details.Add((2, OPEN_PON));
                }
                else
                {
                    fu_details.Add((4, CLOSED_PON));
                }
            }
            var add_tsumo_fu = fu_details.Count > 0 || config.options.fu_for_pinfu_tsumo;
            if (config.is_tsumo && add_tsumo_fu)
            {
                // 2 additional fu for tsumo (but not for pinfu)
                fu_details.Add((2, TSUMO));
            }
            if (is_open_hand && fu_details.Count == 0 && config.options.fu_for_open_pinfu)
            {
                // there is no 1-20 hands, so we have to add additional fu
                fu_details.Add((2, HAND_WITHOUT_FU));
            }
            if (is_open_hand || config.is_tsumo)
            {
                fu_details.Add((20, BASE));
            }
            else
            {
                fu_details.Add((30, BASE));
            }
            return (fu_details, round_fu(fu_details));
        }

        private static int round_fu(List<(int,string)> fu_details)
        {
            // 22 -> 30 and etc.
            var fu = (from x in fu_details
                        select x.Item1).ToList().Sum();
            return (fu + 9) / 10 * 10;
        }
    }
    
}
