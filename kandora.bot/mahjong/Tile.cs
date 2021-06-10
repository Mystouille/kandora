//=====================================================
//=== Code shamelessly converted from   ==============
//= https://github.com/MahjongRepository/mahjong  ===
//====  All credits go to Alexey Nihisil ============
//=====================================================

using System.Collections.Generic;
using System.Linq;
using System;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong
{

    public class Tile
    {

        public object value = null;

        public object is_tsumogiri = null;

        public Tile(object value, object is_tsumogiri)
        {
            this.value = value;
            this.is_tsumogiri = is_tsumogiri;
        }
    }

    public class TilesConverter
    {

        // 
        //         Convert 136 tiles array to the one line string
        //         Example of output with print_aka_dora=False: 1244579m3p57z
        //         Example of output with print_aka_dora=True:  1244079m3p57z
        //         
        public static string to_one_line_string(List<int> tiles, bool print_aka_dora = false)
        {
            tiles = tiles.OrderBy(_p_1 => _p_1).ToList();
            var man = (from t in tiles
                        where 0 < t && t < 36
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
            return sou2 + pin2 + man2 + honors2;
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
        public static int[] to_34_array(int[] tiles)
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
        public static List<int> to_136_array(List<int> tiles)
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

        private static int[] _split_string (string str, int offset, bool has_aka_dora, int red = -1) 
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
        public static int[] string_to_136_array(
            string sou = null,
            string pin = null,
            string man = null,
            string honors = null,
            bool has_aka_dora = false)
        {
                
            IEnumerable<int> results = _split_string(man, 0, has_aka_dora, C.FIVE_RED_MAN);
            results = results.Concat(_split_string(pin, 36, has_aka_dora, C.FIVE_RED_PIN));
            results = results.Concat(_split_string(sou, 72, has_aka_dora, C.FIVE_RED_SOU));
            results = results.Concat(_split_string(honors, 108, has_aka_dora));
            return results.ToArray();
        }

        // 
        //         Method to convert one line string tiles format to the 34 array
        //         We need it to increase readability of our tests
        //         
        public static int[] string_to_34_array(string sou = null, string pin = null, string man = null, string honors = null)
        {
            var results = string_to_136_array(sou, pin, man, honors);
            return to_34_array(results);
        }

        // 
        //         Our shanten calculator will operate with 34 tiles format,
        //         after calculations we need to find calculated 34 tile
        //         in player's 136 tiles.
        // 
        //         For example we had 0 tile from 34 array
        //         in 136 array it can be present as 0, 1, 2, 3
        //         
        public static int find_34_tile_in_136_array(int tile34, List<int> tiles)
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
        public static int[] one_line_string_to_136_array(string str, bool has_aka_dora = false)
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
            return TilesConverter.string_to_136_array(sou, pin, man, honors, has_aka_dora);
        }

        // 
        //         Method to convert one line string tiles format to the 34 array, like
        //         "123s456p789m11222z". 's' stands for sou, 'p' stands for pin,
        //         'm' stands for man and 'z' or 'h' stands for honor.
        //         You can pass r or 0 instead of 5 for it to become a red five from
        //         that suit. To prevent old usage without red,
        //         has_aka_dora has to be True for this to do that.
        //         
        public static int[] one_line_string_to_34_array(string str, bool has_aka_dora = false)
        {
            var results = one_line_string_to_136_array(str, has_aka_dora);
            results = to_34_array(results);
            return results;
        }
    }
}
