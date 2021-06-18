

using System.Collections.Generic;
using System.Linq;
using System;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong
{

    public class TilesConverter
    {

        //  ================================
        //  ====   NAMING CONVENTIONS  =====
        //  ================================
        //
        // Various hand representation are handled throughout this code, here are their meaning depending.
        // Note that they are often named as such, or at least their type is mentioned in the functions metadata
        //
        // Bases:
        // - simple: from 0 to 8 (seldom used)
        // - 34: can represent each tile, without red fives (often used)
        // - 136 can represent every tile, with redv fives, since 2 identical tiles have different values here (often used)

        // Variables:
        //  34count: array of size 34, where array[i] = k means "there is k tiles of index (of base 34) i in this hand"
        //  34Idx  : list of variable size, where "list.Contains(i) means "the tile of index (of base 34) i is in this hand/group"
        //  136    : list of variable size, where "list.Contains(i) means "the tile of index (of base 138) i is in this hand/group"
        //  string : the string representation fo the hand, in 123p123m123s11333z format
        //
        //


        // 
        //         Convert 136 tiles array to the one line string
        //         Example of output with print_aka_dora=False: 1244579m3p57z
        //         Example of output with print_aka_dora=True:  1244079m3p57z
        //         
        public static string ToString(List<int> tiles, bool print_aka_dora = false, string suitOrder= null)
        {
            tiles = tiles.OrderBy(_p_1 => _p_1).ToList();
            var man = (from t in tiles
                        where 0 <= t && t < 36
                        select t).ToList();
            var pin = (from t in tiles
                        where 36 <= t && t < 72
                        select t).ToList();
            pin = (from t in pin
                    select (t - 36)).ToList();
            var sou = (from t in tiles
                        where 72 <= t && t < 108
                        select t).ToList();
            sou = (from t in sou
                    select (t - 72)).ToList();
            var honors = (from t in tiles
                            where t >= 108
                            select t).ToList();
            honors = (from t in honors
                        select (t - 108)).ToList();

            var sou2 = words(sou, C.FIVE_RED_SOU - 72, "s", print_aka_dora);
            var pin2 = words(pin, C.FIVE_RED_PIN - 36, "p", print_aka_dora);
            var man2 = words(man, C.FIVE_RED_MAN, "m", print_aka_dora);
            var honors2 = words(honors, -1 - 108, "z", print_aka_dora);

            if(suitOrder == null)
            {
                return sou2 + pin2 + man2 + honors2;
            }
            var result = "";
            foreach(var chr in suitOrder)
            {
                switch (chr)
                {
                    case 'm':
                        result += man2;
                        break;
                    case 'p':
                        result += pin2;
                        break;
                    case 's':
                        result += sou2;
                        break;
                    case 'z':
                        result += honors2;
                        honors2 = "";
                        break;
                    case 'h':
                        result += honors2;
                        honors2 = "";
                        break;
                }
            }
            return result;
        }
        private static string words(List<int> suits, int red_five, string suffix, bool print_aka_dora)
        {
            if(suits == null || suits.Count == 0)
            {
                return "";
            }
            var suits2 = suits.Select(x => (x == red_five && print_aka_dora) ? "0" : ((x / 4 + 1).ToString()));
            return string.Join("", suits2) + suffix;
        }


        // 
        //         Convert 136 array to the 34 tiles array
        //         
        public static int[] From136to34count(int[] tiles)
        {
            int[] results = new int[34];
            foreach (var tile in tiles)
            {   
                if(tile >= 0)
                {
                    int tile2 = (int)Math.Floor((decimal)(tile / 4));
                    results[tile2] += 1;
                }
            }
            return results;
        }

        // 
        //         Convert 34 array to the 136 tiles array
        //         
        public static List<int> From34countTo136(List<int> tiles)
        {
            var temp = new List<int>();
            var results = new List<int>();
            foreach (var x in Enumerable.Range(0, 34 - 0))
            {
                if (tiles[x]>0)
                {

                    List<int> temp_value = new List<int>(tiles[x]);
                    temp_value.AddRange(Enumerable.Repeat(x * 4, tiles[x]));
                    foreach (var tile in temp_value)
                    {
                        if (results.Contains(tile))
                        {
                            var count_of_tiles = (from z in temp
                                                    where z == tile
                                                    select z).ToList().Count;
                            var new_tile = tile + count_of_tiles;
                            results.Add(new_tile);
                            temp.Add(tile);
                        }
                        else
                        {
                            results.Add(tile);
                            temp.Add(tile);
                        }
                    }
                }
            }
            return results;
        }

        private static int[] SplitString (string str, int offset, bool has_aka_dora, int red = -1) 
            {
            var data = new List<int>();
            var temp = new List<int>();
            if (str == null)
            {
                return new int[] { };
            }
            foreach (var i in str)
            {
                if ((i == 'r' || i == '0') && has_aka_dora)
                {
                    if(red >0)
                    {
                        temp.Add(red);
                        data.Add(red);
                    }
                }
                else
                {
                    int iNum = int.Parse(i.ToString());
                    var tile = offset + (iNum - 1) * 4;
                    if (tile == red && has_aka_dora)
                    {
                        // prevent non reds to become red
                        tile += 1;
                    }
                    if (data.Contains(tile))
                    {
                        var count_of_tiles = (from x in temp
                                                where x == tile
                                                select x).ToList().Count;
                        var new_tile = tile + count_of_tiles;
                        data.Add(new_tile);
                        temp.Add(tile);
                    }
                    else
                    {
                        data.Add(tile);
                        temp.Add(tile);
                    }
                }
            }
            return data.ToArray();
        }

        // 
        //         Method to convert one line string tiles format to the 136 array.
        //         You can pass r or 0 instead of 5 for it to become a red five from
        //         that suit. To prevent old usage without red,
        //         has_aka_dora has to be True for this to do that.
        //         We need it to increase readability of our tests
        //         
        public static int[] FromStringsTo136(
            string sou = null,
            string pin = null,
            string man = null,
            string honors = null,
            bool has_aka_dora = false)
        {
                
            IEnumerable<int> results = SplitString(man, 0, has_aka_dora, C.FIVE_RED_MAN);
            results = results.Concat(SplitString(pin, 36, has_aka_dora, C.FIVE_RED_PIN));
            results = results.Concat(SplitString(sou, 72, has_aka_dora, C.FIVE_RED_SOU));
            results = results.Concat(SplitString(honors, 108, has_aka_dora));
            return results.ToArray();
        }

        // 
        //         Our shanten calculator will operate with 34 tiles format,
        //         after calculations we need to find calculated 34 tile
        //         in player's 136 tiles.
        // 
        //         For example we had 0 tile from 34 array
        //         in 136 array it can be present as 0, 1, 2, 3
        //         
        public static int Find34IdxIn136List(int tile34, List<int> tiles)
        {
            if (tile34 < 0 || tile34 > 33)
            {
                return -1;
            }
            int tile = tile34 * 4;
            var possible_tiles = new List<int> {
                tile
            };
            var otherPossibleTiles =(from i in Enumerable.Range(1, 4 - 1)
                    select (tile + i)).ToList();
            possible_tiles = possible_tiles.Concat(otherPossibleTiles).ToList();
            int found_tile = -1;
            foreach (var possible_tile in possible_tiles)
            {
                if (tiles.Contains(possible_tile))
                {
                    found_tile = possible_tile;
                    break;
                }
            }
            return found_tile;
        }

        // 
        //         Method to convert one line string tiles format to the 136 array, like
        //         "123s456p789m11222z". 's' stands for sou, 'p' stands for pin,
        //         'm' stands for man and 'z' or 'h' stands for honor.
        //         You can pass r or 0 instead of 5 for it to become a red five from
        //         that suit. To prevent old usage without red,
        //         has_aka_dora has to be True for this to do that.
        //         
        public static int[] FromStringTo136(string str, bool has_aka_dora = false)
        {
            var sou = "";
            var pin = "";
            var man = "";
            var honors = "";
            var split_start = 0;
            foreach (var _tup_1 in str.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)))
            {
                var index = _tup_1.Item1;
                var i = _tup_1.Item2;
                var length = index - split_start;
                var subStr = str.Substring(split_start, length);
                if (i == 'm')
                {
                    man += subStr;
                    split_start = index + 1;
                }
                if (i == 'p')
                {
                    pin += subStr;
                    split_start = index + 1;
                }
                if (i == 's')
                {
                    sou += subStr;
                    split_start = index + 1;
                }
                if (i == 'z' || i == 'h')
                {
                    honors += subStr;
                    split_start = index + 1;
                }
            }
            return TilesConverter.FromStringsTo136(sou, pin, man, honors, has_aka_dora);
        }

        public static int[] FromStringTo34Count(string str, bool has_aka_dora = false)
        {
            var results = FromStringTo136(str, has_aka_dora);
            results = From136to34count(results);
            return results;
        }

        public static int[] FromStringsTo34Count(string sou = null, string pin = null, string man = null, string honors = null)
        {
            var results = FromStringsTo136(sou, pin, man, honors);
            return From136to34count(results);
        }


        public static string From34DdxHandToString(List<List<int>> hand)
        {
            var array = From34IxdHandTo136(hand);
            return ToString(array);
        }

        public static List<int> From34IxdHandTo136(List<List<int>> hand)
        {
            List<int> array136 = new List<int>();
            var fullArray34 = new int[34];
            foreach (var set in hand)
            {
                var array34 = new int[34];
                foreach (var tile in set)
                {
                    array34[tile]++;
                }
                for (int idx = 0; idx < array34.Length; idx++)
                {
                    int nbT = fullArray34[idx];
                    for (int i = nbT; i < array34[idx]; i++)
                    {
                        array136.Add(idx * 4 + i);
                    }
                    fullArray34[idx] += nbT;
                }
            }
            return array136;
        }

        public static List<List<int>> From34IdxHandTo136Hand(List<List<int>> hand)
        {
            List<List<int>> arrays136 = new List<List<int>>();
            var fullArray34 = new int[34];
            foreach (var set in hand)
            {
                var array34 = new int[34];
                List<int> array136 = new List<int>();
                foreach (var tile in set)
                {
                    array34[tile]++;
                }
                for (int idx = 0; idx < array34.Length; idx++)
                {
                    int nbT = fullArray34[idx];
                    for (int i = nbT; i < array34[idx]; i++)
                    {
                        array136.Add(idx * 4 + i);
                    }
                    fullArray34[idx] += nbT;
                }
                arrays136.Add(array136);

            }
            arrays136 = arrays136.OrderBy(x => {
                float sum = x.Aggregate((x, y) => x + y);
                return sum/x.Count();
            }).ToList();
            return arrays136;
        }



    }
}
