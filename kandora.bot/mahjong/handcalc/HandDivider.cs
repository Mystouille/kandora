namespace kandora.bot.mahjong.handcalc
{

    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Text;
    using C = kandora.bot.mahjong.Constants;
    using Converter = kandora.bot.mahjong.TilesConverter;
    using U = kandora.bot.mahjong.Utils;
    using kandora.bot.utils;

    public class HandDivider
    {

        Dictionary<string, List<List<List<int>>>> divider_cache = null;

        string cache_key = null;

        public HandDivider()
        {
            divider_cache = new Dictionary<string, List<List<List<int>>>>();
        }

        // 
        //         Return a list of possible hands.
        //         :param tiles_34:
        //         :param melds: list of Meld objects
        //         :return:
        //         
        public virtual List<List<List<int>>> divide_hand(int[] tiles_34, List<Meld> melds = null, bool use_cache = false)
        {
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            if (use_cache)
            {
                cache_key = _build_divider_cache_key(tiles_34, melds);
                if (divider_cache.ContainsKey(cache_key))
                {
                    return divider_cache[cache_key];
                }
            }
            var closed_hand_tiles_34 = new int[34];
            tiles_34.CopyTo(closed_hand_tiles_34, 0);
            // small optimization, we can't have a pair in open part of the hand,
            // so we don't need to try find pairs in open sets
            IEnumerable<IEnumerable<int>> open_tile_indices_list = (from x in melds
                                          select x.tiles_34).ToList();
            IEnumerable<int> open_tile_indices = new List<int>();
            if (melds.Count != 0)
            {
                open_tile_indices = open_tile_indices_list.Aggregate((x, y) => (x.Concat(y)));
            }
            foreach (var open_item in open_tile_indices)
            {
                closed_hand_tiles_34[open_item] -= 1;
            }
            var pair_indices = find_pairs(closed_hand_tiles_34);
            // let's try to find all possible hand options
            IEnumerable<List<List<int>>> hands = new List<List<List<int>>>();
            foreach (var pair_index in pair_indices)
            {
                var local_tiles_34 = new int[34];
                tiles_34.CopyTo(local_tiles_34, 0);
                // we don't need to combine already open sets
                foreach (var open_item in open_tile_indices)
                {
                    local_tiles_34[open_item] -= 1;
                }
                local_tiles_34[pair_index] -= 2;
                // 0 - 8 man tiles
                var man = find_valid_combinations(local_tiles_34, 0, 8);
                // 9 - 17 pin tiles
                var pin = find_valid_combinations(local_tiles_34, 9, 17);
                // 18 - 26 sou tiles
                var sou = find_valid_combinations(local_tiles_34, 18, 26);
                var honor1 = new List<List<int>>();
                foreach (var x in C.HONOR_INDICES)
                {
                    if (local_tiles_34[x] == 3)
                    {
                        honor1.Add(Enumerable.Repeat(x, 3).ToList());
                    }
                }
                var honor = new List<List<List<int>>>();
                honor.Add(honor1);


                var pair = new List<int> {
                    pair_index,pair_index
                };

                var tempHands = from p in pin
                    from m in man
                    from s in sou
                    from h in honor
                    select p.Concat(m).Concat(s).Concat(h).Concat(melds.Select(x=>x.tiles)).Append(pair).ToList();
                hands = hands.Concat(tempHands.Where(x => x.Count == 5).OrderBy(x => x[0]));
            }
            // small optimization, let's remove hand duplicates
            var unique_hands = new List<List<List<int>>>();
            foreach (var hand in hands)
            {
                var hand136 = Converter.from_34_indices_to_136_arrays(hand);
                var readable = hand136.Select(x => Converter.to_one_line_string(x));
                var str = string.Join("|", readable);
                bool isOk = true;
                foreach (var otherHand in unique_hands)
                {
                    var handListStr1 = string.Join('.',hand.Aggregate((x, y) => x.Concat(y).ToList()).Select(y=>y.ToString()));
                    var handListStr2 = string.Join('.', otherHand.Aggregate((x, y) => x.Concat(y).ToList()).Select(y => y.ToString()));
                    isOk &= (handListStr1 != handListStr2);
                }
                if (isOk)
                {
                    unique_hands.Add(hand);
                }
            }
            hands = unique_hands;

            if (pair_indices.Count == 7)
            {
                var hand = new List<List<int>>();
                foreach (var index in pair_indices)
                {
                    hand.Add(new List<int> {
                        index, index
                    });
                }
                hands = hands.Append(hand);
            }
            var listHands = hands.ToList();
            if (use_cache)
            {
                this.divider_cache[this.cache_key] = listHands;
            }
            return listHands;
        }

        // 
        //         Find all possible pairs in the hand and return their indices
        //         :return: array of pair indices
        //         
        private List<int> find_pairs(int[] tiles_34, int first_index = 0, int second_index = 33)
        {
            var pair_indices = new List<int>();
            foreach (var x in Enumerable.Range(first_index, second_index + 1 - first_index))
            {
                // ignore pon of honor tiles, because it can't be a part of pair
                if (C.HONOR_INDICES.Contains(x) && tiles_34[x] != 2)
                {
                    continue;
                }
                if (tiles_34[x] >= 2)
                {
                    pair_indices.Add(x);
                }
            }
            return pair_indices;
        }

        static IEnumerable<IEnumerable<T>> Permute<T> (IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                yield break;
            }

            var list = sequence.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                var startingElementIndex = 0;

                foreach (var startingElement in list)
                {
                    var index = startingElementIndex;
                    var remainingItems = list.Where((e, i) => i != index);

                    foreach (var permutationOfRemainder in Permute(remainingItems))
                    {
                        yield return permutationOfRemainder.Prepend(startingElement);
                    }

                    startingElementIndex++;
                }
            }
        }

        // Replaced the original "get possible sets" block by this, seems more efficient and compact
        // Also: binary count browsing is cool
        // Input: sorted array of indices
        private static List<List<int>> GetPossibleSetsFromIndices(List<int> list)
        {
            var array = list.ToArray();
            int count = (int)Math.Pow(2, array.Length); //We know we'll iterate on 2^n possibilities
            int min = (int)Math.Pow(2, 3) - 1; //start at b'...000000111' since it's the first result of length >= 3
            int max = count - (int)Math.Pow(2, array.Length - 3); //end at b'11100000...' since it's the last result of length <= 3 (12% faster)
            var results = new List<List<int>>();
            if( list.Count < 3)
            {
                return results;
            }
            int lastTile = -1;
            int currentTile;
            int nbTile;
            bool ok;
            for (int i = min; i <= max ; i++)
            {
                var result = new List<int>();
                string str = Convert.ToString(i, 2).PadLeft(array.Length, '0'); //we take i and convert it to a n bit integer, each value of i representing a possibility
                nbTile = 0;
                ok = true;
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        if (nbTile == 3)
                        {
                            ok = false;
                            break; // result is too large so let's stop here (might be the only useful optim here...)
                        }
                        currentTile = array[j];
                        if (currentTile - lastTile > 1 && lastTile >= 0)
                        {
                            ok = false;
                            break; // there is no way a group containing 'x,x+2' is a set
                        }
                        result.Add(currentTile);
                        lastTile = currentTile;
                        nbTile++;
                    }
                }
                if (ok && (U.is_chi(result) || U.is_pon(result)))
                {
                    results.Add(result);
                }
            }
            return results;
        }

        private static List<List<int>> GetPossibleSetsFromIndices2(List<int> list)
        {
            var results = new List<List<int>>();
            for (int t1 = 0; t1 < list.Count; t1++)
            {
                for (int t2 = t1; t2 < list.Count; t2++)
                {
                    if (t2 == t1)
                    {
                        continue;
                    }
                    for (int t3 = t2; t3 < list.Count; t3++)
                    {
                        if (t3 == t2 || t3 == t1)
                        {
                            continue;
                        }
                        var result = new List<int> { list[t1], list[t2], list[t3] };
                        if (U.is_chi(result) || U.is_pon(result))
                        {
                            results.Add(result);
                        }
                    }
                }
            }
            return results;
        }

        //Same strategy as GetPossibleSetsFromIndices
        // input: list: the list of possible sets, indexCount: indexCount[i] is the number of tiles with index i in the hand
        static List<List<List<int>>> GetPossibleHandsFromSets(List<List<int>> list, int[] indexCount, int maxNbSets)
        {
            var array = list.ToArray();
            int count = (int)Math.Pow(2, list.Count);
            int min = (int)Math.Pow(2, maxNbSets) -1; //start at b'...000001111' since it's the first result of length >= maxNbMelds
            int max = count - (int)Math.Pow(2, list.Count - maxNbSets); //end at b'11110000...' since it's the last result of length <= maxNbMelds
            var results = new List<List<List<int>>>();
            int nbMelds;
            bool ok;
            for (int i = min; i <= max; i++)
            {
                nbMelds = 0;
                ok = true;
                var result = new List<List<int>>();
                var resultIndexCount = new int[indexCount.Length];
                string str = Convert.ToString(i, 2).PadLeft(array.Length, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        if (nbMelds == maxNbSets)
                        {
                            ok = false;
                            break; // result is too large so let's stop here (might be the only useful optim here...)
                        }
                        result.Add(array[j]);
                        nbMelds++;
                    }
                }
                if (ok)
                {
                    result.ForEach(x => x.ForEach(y => resultIndexCount[y]++));
                    // A hand resulting from a specific set combination is valid IF AND ONLY IF it's similar to the input hand
                    if (indexCount.SequenceEqual(resultIndexCount))
                    {
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        // 
        //         Find and return all valid set combinations in given suit
        //         :param tiles_34:
        //         :param first_index:
        //         :param second_index:
        //         :param hand_not_completed: in that mode we can return just possible shi or pon sets
        //         :return: list of valid combinations
        //         
        List<List<List<int>>> find_valid_combinations(int[] tiles_34, int first_index, int second_index, bool hand_not_completed = false)
        {
            int count_of_sets;
            var indices = new List<int>();
            foreach (var x in Enumerable.Range(first_index, second_index + 1 - first_index))
            {
                if (tiles_34[x] > 0)
                {
                    indices.AddRange(Enumerable.Repeat(x, tiles_34[x]).ToList());
                }
            }
            var count_of_needed_combinations = Convert.ToInt32(indices.Count / 3);
            if (indices.Count==0 || count_of_needed_combinations == 0)
            {
                return new List<List<List<int>>>
                {
                    new List<List<int>>()
                };
            }

            // indices are already sorted
            var validMelds = GetPossibleSetsFromIndices(indices);
            if (validMelds.Count == 0)
            {
                return new List<List<List<int>>>
                {
                    new List<List<int>>()
                };
            }
            // simple case, we have count of sets == count of tiles
            if (count_of_needed_combinations == validMelds.Count) {
                var toCheck = validMelds.Select(x=>x.ToList() as IEnumerable<int>).Aggregate((z, y) => z.Concat(y));
                if (toCheck.SequenceEqual(indices))
                {
                    return new List<List<List<int>>>() {
                        validMelds
                    };
                }
            }

            // filter and remove not possible pon sets
            IEnumerable<List<int>> identicalSets;
            foreach (var item in validMelds.ToArray().ToList())
            {
                if (U.is_pon(item))
                {
                    count_of_sets = 1;
                    var count_of_tiles = 0;
                    while (count_of_sets > count_of_tiles)
                    {
                        count_of_tiles = (from x in indices
                                            where x == item[0]
                                            select x).ToList().Count / 3;
                        identicalSets = (from x in validMelds
                                         where x[0] == item[0] && x[1] == item[1] && x[2] == item[2]
                                         select x);
                        count_of_sets = identicalSets.Count();
                        if (count_of_sets > count_of_tiles)
                        {
                            validMelds.Remove(identicalSets.First());
                        }
                    }
                }
            }
            // filter and remove not possible chi sets
            foreach (var item in validMelds.ToArray().ToList())
            {
                if (U.is_chi(item))
                {
                    count_of_sets = 5;
                    // TODO calculate real count of possible sets
                    var count_of_possible_sets = 4;
                    while (count_of_sets > count_of_possible_sets)
                    {
                        identicalSets = (from x in validMelds
                                            where x[0] == item[0] && x[1] == item[1] && x[2] == item[2]
                                            select x);
                        count_of_sets = identicalSets.Count();
                        if (count_of_sets > count_of_possible_sets)
                        {
                            validMelds.Remove(identicalSets.First());
                        }
                    }
                }
            }
            // lit of chi\pon sets for not completed hand
            if (hand_not_completed)
            {
                return new List<List<List<int>>>() {
                        validMelds
                };
            }
            // hard case - we can build a lot of sets from our tiles
            // for example we have 123456 tiles and we can build sets:
            // [1, 2, 3] [4, 5, 6] [2, 3, 4] [3, 4, 5]
            // and only two of them valid in the same time [1, 2, 3] [4, 5, 6]

            int maxIndex = indices.Max()+1;
            var indexCount = new int[maxIndex];
            foreach(var index in indices)
            {
                indexCount[index]++;
            }
            var possibleHands = GetPossibleHandsFromSets(validMelds, indexCount, count_of_needed_combinations);
            return possibleHands;
        }

        private void clear_cache()
        {
            divider_cache = new Dictionary<string, List<List<List<int>>>>();
            cache_key = null;
        }

        private string _build_divider_cache_key(int[] tiles_34, List<Meld> melds)
        {
            var array = tiles_34.ToList();
            array.Add(-1);
            var prepared_array = array.Concat(melds.Select(x => x.tiles).Aggregate((x, y) => x.Concat(y).ToList())).ToArray();
            return CreateMD5(prepared_array);
        }

        private static string CreateMD5(int[] input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = new byte[input.Length * sizeof(int)];
                Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}