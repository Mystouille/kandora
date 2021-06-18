namespace kandora.bot.mahjong.handcalc
{

    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Text;
    using C = kandora.bot.mahjong.Constants;
    using Converter = kandora.bot.mahjong.TilesConverter;
    using U = kandora.bot.mahjong.Utils;

    public class HandDivider
    {

        Dictionary<string, List<List<List<int>>>> dividerCache = null;

        string cacheKey = null;

        public HandDivider()
        {
            dividerCache = new Dictionary<string, List<List<List<int>>>>();
        }

        /// <summary>
        /// Return a list of all possible interpretations for a given hand
        /// </summary>
        /// <param name="tiles">34count format of the hand (</param>
        /// <param name="melds">The list of the melds</param>
        /// <param name="useCache">True: Do not compute the same hand twice</param>
        /// <returns>The list of all possible hands</returns>
        public virtual List<List<List<int>>> DivideHand(int[] tiles34, List<Meld> melds = null, bool useCache = false)
        {
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            if (useCache)
            {
                cacheKey = BuildDividerCachekey(tiles34, melds);
                if (dividerCache.ContainsKey(cacheKey))
                {
                    return dividerCache[cacheKey];
                }
            }
            var closedHandTiles34 = new int[34];
            tiles34.CopyTo(closedHandTiles34, 0);
            // small optimization, we can't have a pair in open part of the hand,
            // so we don't need to try find pairs in open sets
            IEnumerable<IEnumerable<int>> openTileIndicesList = (from x in melds
                                          select x.Tiles34).ToList();
            IEnumerable<int> openTileIndices = new List<int>();
            if (melds.Count != 0)
            {
                openTileIndices = openTileIndicesList.Aggregate((x, y) => (x.Concat(y)));
            }
            foreach (var item in openTileIndices)
            {
                closedHandTiles34[item] -= 1;
            }
            var pairIndices = FindPairs(closedHandTiles34);
            // let's try to find all possible hand options
            IEnumerable<List<List<int>>> hands = new List<List<List<int>>>();
            foreach (var pairIndex in pairIndices)
            {
                var localTiles = new int[34];
                tiles34.CopyTo(localTiles, 0);
                // we don't need to combine already open sets
                foreach (var item in openTileIndices)
                {
                    localTiles[item] -= 1;
                }
                localTiles[pairIndex] -= 2;
                // 0 - 8 man tiles
                var man = FindValidCombinations(localTiles, 0, 8);
                // 9 - 17 pin tiles
                var pin = FindValidCombinations(localTiles, 9, 17);
                // 18 - 26 sou tiles
                var sou = FindValidCombinations(localTiles, 18, 26);
                var honor1 = new List<List<int>>();
                foreach (var x in C.HONOR_INDICES)
                {
                    if (localTiles[x] == 3)
                    {
                        honor1.Add(Enumerable.Repeat(x, 3).ToList());
                    }
                }
                var honor = new List<List<List<int>>>();
                honor.Add(honor1);


                var pair = new List<int> {
                    pairIndex,pairIndex
                };

                var tempHands = from p in pin
                    from m in man
                    from s in sou
                    from h in honor
                    select p.Concat(m).Concat(s).Concat(h).Concat(melds.Select(x=>x.tiles)).Append(pair).ToList();
                hands = hands.Concat(tempHands.Where(x => x.Count == 5).OrderBy(x => x[0]));
            }
            // small optimization, let's remove hand duplicates
            var uniqueHands = new List<List<List<int>>>();
            foreach (var hand in hands)
            {
                var hand136 = Converter.From34IdxHandTo136Hand(hand);
                var readable = hand136.Select(x => Converter.ToString(x));
                var str = string.Join("|", readable);
                bool isOk = true;
                foreach (var otherHand in uniqueHands)
                {
                    var handListStr1 = string.Join('.',hand.Aggregate((x, y) => x.Concat(y).ToList()).Select(y=>y.ToString()));
                    var handListStr2 = string.Join('.', otherHand.Aggregate((x, y) => x.Concat(y).ToList()).Select(y => y.ToString()));
                    isOk &= (handListStr1 != handListStr2);
                }
                if (isOk)
                {
                    uniqueHands.Add(hand);
                }
            }
            hands = uniqueHands;

            if (pairIndices.Count == 7)
            {
                var hand = new List<List<int>>();
                foreach (var index in pairIndices)
                {
                    hand.Add(new List<int> {
                        index, index
                    });
                }
                hands = hands.Append(hand);
            }
            var listHands = hands.ToList();
            if (useCache)
            {
                this.dividerCache[this.cacheKey] = listHands;
            }
            return listHands;
        }

        /// <summary>
        /// Find the list of all possible pairs in the part of a hand and return their 34idx format
        /// </summary>
        /// <param name="tiles34">34count format of one suit of the hand</param>
        /// <param name="firstIndex">The first 34idx value to search pairs in</param>
        /// <param name="secondIndex">The last 34idx value to search pairs in</param>
        /// <returns>A list of 34idx format</returns>
        private List<int> FindPairs(int[] tiles34, int firstIndex = 0, int secondIndex = 33)
        {
            var pairIndices = new List<int>();
            foreach (var x in Enumerable.Range(firstIndex, secondIndex + 1 - firstIndex))
            {
                // ignore pon of honor tiles, because it can't be a part of pair
                if (C.HONOR_INDICES.Contains(x) && tiles34[x] != 2)
                {
                    continue;
                }
                if (tiles34[x] >= 2)
                {
                    pairIndices.Add(x);
                }
            }
            return pairIndices;
        }

        /// <summary>
        /// Find and return all valid set combinations in given suit
        /// </summary>
        /// <param name="tiles34"> The 34count format of the original hand (one suit only)</param>
        /// <param name="firstIndex">The 34idx start of the suit</param>
        /// <param name="secondIndex">The 34idx end of the suit</param>
        /// <param name="handNotCompleted"> True if the hand is not a winning one</param>
        /// <returns>A list of the posible set combinations for this suit, for this hand, in 34idx format</returns>
        List<List<List<int>>> FindValidCombinations(int[] tiles34, int firstIndex, int secondIndex, bool handNotCompleted = false)
        {
            int countOfSets;
            var indices = new List<int>();
            foreach (var x in Enumerable.Range(firstIndex, secondIndex + 1 - firstIndex))
            {
                if (tiles34[x] > 0)
                {
                    indices.AddRange(Enumerable.Repeat(x, tiles34[x]).ToList());
                }
            }
            var countOfNeededCombinations = Convert.ToInt32(indices.Count / 3);
            if (indices.Count==0 || countOfNeededCombinations == 0)
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
            if (countOfNeededCombinations == validMelds.Count) {
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
                if (U.IsKoutsu(item))
                {
                    countOfSets = 1;
                    var count_of_tiles = 0;
                    while (countOfSets > count_of_tiles)
                    {
                        count_of_tiles = (from x in indices
                                            where x == item[0]
                                            select x).ToList().Count / 3;
                        identicalSets = (from x in validMelds
                                         where x[0] == item[0] && x[1] == item[1] && x[2] == item[2]
                                         select x);
                        countOfSets = identicalSets.Count();
                        if (countOfSets > count_of_tiles)
                        {
                            validMelds.Remove(identicalSets.First());
                        }
                    }
                }
            }
            // filter and remove not possible chi sets
            foreach (var item in validMelds.ToArray().ToList())
            {
                if (U.IsShuntsu(item))
                {
                    countOfSets = 5;
                    // TODO calculate real count of possible sets
                    var countOfPossibleSets = 4;
                    while (countOfSets > countOfPossibleSets)
                    {
                        identicalSets = (from x in validMelds
                                            where x[0] == item[0] && x[1] == item[1] && x[2] == item[2]
                                            select x);
                        countOfSets = identicalSets.Count();
                        if (countOfSets > countOfPossibleSets)
                        {
                            validMelds.Remove(identicalSets.First());
                        }
                    }
                }
            }
            // lit of chi\pon sets for not completed hand
            if (handNotCompleted)
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
            var possibleHands = GetPossibleHandsFromSets(validMelds, indexCount, countOfNeededCombinations);
            return possibleHands;
        }



        /// <summary>
        /// Get the list of possible sets from a single suit
        /// </summary>
        /// <param name="list"> The list of the tiles in a specific suit, in 34idx format (</param>
        /// <returns>A list of the posible sets</returns>
        private static List<List<int>> GetPossibleSetsFromIndices(List<int> list)
        {
            var array = list.ToArray();
            var results = new List<List<int>>();
            for (int t1 = 0; t1 < array.Length; t1++)
            {
                for (int t2 = t1; t2 < array.Length; t2++)
                {
                    if (t2 == t1)
                    {
                        continue;
                    }
                    for (int t3 = t2; t3 < array.Length; t3++)
                    {
                        if (t3 == t2 || t3 == t1)
                        {
                            continue;
                        }
                        var result = new List<int> { array[t1], array[t2], array[t3] };
                        if (U.IsShuntsu(result) || U.IsKoutsu(result))
                        {
                            results.Add(result);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Get the list of possible set combinations of a single suit from a list of the possible sets
        /// </summary>
        /// <param name="list"> A list of the posible sets of a specific suit</param>
        /// <param name="indexCount"> The 34count format of the original hand (one suit only)</param>
        /// <param name="maxNbSets"> The max number of set of this suit the hand can have</param>
        /// <returns>A list of the posible set combinations that are compatible with the hand, in 34idx format</returns>
        static List<List<List<int>>> GetPossibleHandsFromSets(List<List<int>> list, int[] indexCount, int maxNbSets)
        {
            var array = list.ToArray();
            int count = (int)Math.Pow(2, list.Count);
            int min = (int)Math.Pow(2, maxNbSets) - 1; //start at b'...000001111' since it's the first result of length >= maxNbMelds
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

        private string BuildDividerCachekey(int[] tiles34, List<Meld> melds)
        {
            var array = tiles34.ToList();
            array.Add(-1);
            var preparedArray = array.Concat(melds.Select(x => x.tiles).Aggregate((x, y) => x.Concat(y).ToList())).ToArray();
            return CreateMD5(preparedArray);
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