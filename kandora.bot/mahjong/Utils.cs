namespace kandora.bot.mahjong
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using C = kandora.bot.mahjong.Constants;

    public static class Utils
    {

        // 
        //     Check if tile is aka dora
        //     
        public static bool is_aka_dora(int tile_136, bool aka_enabled)
        {
            if (!aka_enabled)
            {
                return false;
            }
            if (new List<object> {
                C.FIVE_RED_MAN,
                C.FIVE_RED_PIN,
                C.FIVE_RED_SOU
            }.Contains(tile_136))
            {
                return true;
            }
            return false;
        }

        // 
        //     Calculate the number of dora for the tile
        //     
        public static int plus_dora(int tile_136, List<int> dora_indicators_136, bool add_aka_dora = false)
        {
            var tile_index = tile_136 / 4;
            var dora_count = 0;
            if (add_aka_dora && is_aka_dora(tile_136, aka_enabled: true))
            {
                dora_count += 1;
            }
            foreach (var doraIt in dora_indicators_136)
            {
                var dora = doraIt;
                dora /= 4;
                // sou, pin, man
                if (tile_index < C.EAST)
                {
                    // with indicator 9, dora will be 1
                    if (dora == 8)
                    {
                        dora = -1;
                    }
                    else if (dora == 17)
                    {
                        dora = 8;
                    }
                    else if (dora == 26)
                    {
                        dora = 17;
                    }
                    if (tile_index == dora + 1)
                    {
                        dora_count += 1;
                    }
                }
                else
                {
                    if (dora < C.EAST)
                    {
                        continue;
                    }
                    dora -= 9 * 3;
                    var tile_index_temp = tile_index - 9 * 3;
                    // dora indicator is north
                    if (dora == 3)
                    {
                        dora = -1;
                    }
                    // dora indicator is hatsu
                    if (dora == 6)
                    {
                        dora = 3;
                    }
                    if (tile_index_temp == dora + 1)
                    {
                        dora_count += 1;
                    }
                }
            }
            return dora_count;
        }

        // 
        //     :param item: list of tile 34 indices
        //     :return: boolean
        //     
        public static bool is_chi(List<int> list)
        {
            if (list.Count != 3)
            {
                return false;
            }
            return list[0] == list[1] - 1 && list[0] == list[2] - 2;
        }

        // 
        //     :param item: list of tile 34 indices
        //     :return: boolean
        //     
        public static bool is_pon(List<int> list)
        {
            if (list.Count != 3)
            {
                return false;
            }
            return list[0] == list[1] && list[0] == list[2];
        }

        public static bool is_kan(List<int> list)
        {
            return list.Count == 4;
        }

        public static bool is_pon_or_kan(List<int> list)
        {
            return is_pon(list) || is_kan(list);
        }

        // 
        //     :param item: array of tile 34 indices
        //     :return: boolean
        //     
        public static bool is_pair(List<int> item)
        {
            return item.Count == 2;
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_man(int tile)
        {
            return tile <= 8;
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_pin(int tile)
        {
            return 8 < tile && tile <= 17;
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_sou(int tile)
        {
            return 17 < tile && tile <= 26;
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_honor(int tile)
        {
            return tile >= 27;
        }

        public static bool is_sangenpai(int tile_34)
        {
            return tile_34 >= 31;
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_terminal(int tile)
        {
            return C.TERMINAL_INDICES.Contains(tile);
        }

        // 
        //     :param tile: 34 tile format
        //     :return: boolean
        //     
        public static bool is_dora_indicator_for_terminal(int tile)
        {
            return tile == 7 || tile == 8 || tile == 16 || tile == 17 || tile == 25 || tile == 26;
        }

        // 
        //     :param hand_set: array of 34 tiles
        //     :return: boolean
        //     
        public static bool contains_terminals(List<int> hand_set)
        {
            return (from x in hand_set
                        select C.TERMINAL_INDICES.Contains(x)).ToList().Any();
        }

        // 
        //     :param tile: 34 tile format
        //     :return: tile: 0-8 presentation
        //     
        public static int simplify(int tile)
        {
            return tile - 9 * (tile / 9);
        }

        // 
        //     Tiles that don't have -1, 0 and +1 neighbors
        //     :param hand_34: array of tiles in 34 tile format
        //     :return: array of isolated tiles indices
        //     
        public static Stack<int> find_isolated_tile_indices(int[] hand_34)
        {
            var isolated_indices = new Stack<int>();
            foreach (var x in Enumerable.Range(0, C.CHUN + 1 - 0))
            {
                // for honor tiles we don't need to check nearby tiles
                if (is_honor(x) && hand_34[x] == 0)
                {
                    isolated_indices.Push(x);
                }
                else
                {
                    var simplified = simplify(x);
                    // 1 suit tile
                    if (simplified == 0)
                    {
                        if (hand_34[x] == 0 && hand_34[x + 1] == 0)
                        {
                            isolated_indices.Push(x);
                        }
                    }
                    else if (simplified == 8)
                    {
                        // 9 suit tile
                        if (hand_34[x] == 0 && hand_34[x - 1] == 0)
                        {
                            isolated_indices.Push(x);
                        }
                    }
                    else
                    {
                        // 2-8 tiles tiles
                        if (hand_34[x] == 0 && hand_34[x - 1] == 0 && hand_34[x + 1] == 0)
                        {
                            isolated_indices.Push(x);
                        }
                    }
                }
            }
            return isolated_indices;
        }

        // 
        //     Tile is strictly isolated if it doesn't have -2, -1, 0, +1, +2 neighbors
        //     :param hand_34: array of tiles in 34 tile format
        //     :param tile_34: int
        //     :return: bool
        //     
        public static bool is_tile_strictly_isolated(List<int> hand_34, int tile_34)
        {
            if (is_honor(tile_34))
            {
                return hand_34[tile_34] - 1 <= 0;
            }
            var simplified = simplify(tile_34);
            List<int> indices = new List<int>();
            // 1 suit tile
            if (simplified == 0)
            {
                indices = new List<int> {
                    tile_34,
                    tile_34 + 1,
                    tile_34 + 2
                };
            }
            else if (simplified == 1)
            {
                // 2 suit tile
                indices = new List<int> {
                    tile_34 - 1,
                    tile_34,
                    tile_34 + 1,
                    tile_34 + 2
                };
            }
            else if (simplified == 7)
            {
                // 8 suit tile
                indices = new List<int> {
                    tile_34 - 2,
                    tile_34 - 1,
                    tile_34,
                    tile_34 + 1
                };
            }
            else if (simplified == 8)
            {
                // 9 suit tile
                indices = new List<int> {
                    tile_34 - 2,
                    tile_34 - 1,
                    tile_34
                };
            }
            else
            {
                // 3-7 tiles tiles
                indices = new List<int> {
                    tile_34 - 2,
                    tile_34 - 1,
                    tile_34,
                    tile_34 + 1,
                    tile_34 + 2
                };
            }
            var isolated = true;
            foreach (var tile_index in indices)
            {
                // we don't want to count our tile as it is in hand already
                if (tile_index == tile_34)
                {
                    isolated |= hand_34[tile_index] - 1 <= 0;
                }
                else
                {
                    isolated |= hand_34[tile_index] == 0;
                }
            }
            return isolated;
        }

        // 
        //     Separate tiles by suits and count them
        //     :param tiles_34: array of tiles to count
        //     :return: dict
        //     
        public static Dictionary<string, int> count_tiles_by_suits(List<int> tiles_34)
        {
            var suits = new Dictionary<string, int>();
            suits.Add("sou", 0);
            suits.Add("man", 0);
            suits.Add("pin", 0);
            suits.Add("honor", 0);

            foreach (var x in Enumerable.Range(0, 34 - 0))
            {
                var tile = tiles_34[x];
                if (tile == 0)
                {
                    continue;
                }
                suits["sou"] += (is_sou(x) ? tile : 0);
                suits["man"] += (is_man(x) ? tile : 0);
                suits["pin"] += (is_pin(x) ? tile : 0);
                suits["honor"] += (is_honor(x) ? tile : 0);
            }
            return suits;
        }

        // Extract the suit order
        public static string getSuitOrder(string str)
        {
            string order = "";
            var addedChar = new HashSet<char>();
            foreach (var chr in str)
            {
                if ((chr == 'm' || chr == 'p' || chr == 's' || chr == 'z' || chr == 'h') && !addedChar.Contains(chr))
                {
                    order += chr;
                    addedChar.Add(chr);
                }
            }
            return order;
        }

        public static bool are_tiles_in_indices(List<int> set, IEnumerable<int> indices)
        {
            foreach (var tile in set)
            {
                if (!indices.Contains(tile))
                {
                    return false;
                }
            }
            return true;
        }

    }
}