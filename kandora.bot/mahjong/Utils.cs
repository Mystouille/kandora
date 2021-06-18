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
        public static bool IsAkaDora(int tile_136, bool aka_enabled)
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


        /// <summary>
        /// Calculate the number of dora for the tile
        /// </summary>
        /// <param name="tile136"> The hand, in 136 format</param>
        /// <param name="doraIndicators136"> The 136 format list of dora indicators</param>
        /// <param name="addAkaDora"> True if aka doras must be counted</param>
        /// <returns>The number of doras of the hand</returns>   
        public static int PlusDora(int tile136, List<int> doraIndicators136, bool addAkaDora = false)
        {
            var tileIndex = tile136 / 4;
            var doraCount = 0;
            if (addAkaDora && IsAkaDora(tile136, aka_enabled: true))
            {
                doraCount += 1;
            }
            foreach (var doraIt in doraIndicators136)
            {
                var dora = doraIt;
                dora /= 4;
                // sou, pin, man
                if (tileIndex < C.EAST)
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
                    if (tileIndex == dora + 1)
                    {
                        doraCount += 1;
                    }
                }
                else
                {
                    if (dora < C.EAST)
                    {
                        continue;
                    }
                    dora -= 9 * 3;
                    var tile_index_temp = tileIndex - 9 * 3;
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
                        doraCount += 1;
                    }
                }
            }
            return doraCount;
        }

        /// <summary>
        /// Checks if the group is a shuntsu
        /// </summary>
        /// <param name="list"> The group, in 136 format</param>
        /// <returns>True if the group is a shuntsu</returns>   
        public static bool IsShuntsu(List<int> list)
        {
            if (list.Count != 3)
            {
                return false;
            }
            return list[0] == list[1] - 1 && list[0] == list[2] - 2;
        }

        /// <summary>
        /// Checks if the group is a koutsu
        /// </summary>
        /// <param name="list"> The group, in 136 format</param>
        /// <returns>True if the group is a koutsu</returns>   
        public static bool IsKoutsu(List<int> list)
        {
            if (list.Count != 3)
            {
                return false;
            }
            return list[0] == list[1] && list[0] == list[2];
        }

        /// <summary>
        /// Checks if the group is a kantsu
        /// </summary>
        /// <param name="list"> The group, in 136 format</param>
        /// <returns>True if the group is a kantsu</returns>   
        public static bool IsKantsu(List<int> list)
        {
            return list.Count == 4;
        }

        /// <summary>
        /// Checks if the group is a koutsu or kantsu
        /// </summary>
        /// <param name="list"> The group, in 136 format</param>
        /// <returns>True if the group is a koutsu or kantsu</returns>   
        public static bool IsKoutsuOrKantsu(List<int> list)
        {
            return IsKoutsu(list) || IsKantsu(list);
        }

        /// <summary>
        /// Checks if the group is a pair
        /// </summary>
        /// <param name="list"> The group, in 136 format</param>
        /// <returns>True if the group is a pair</returns>   
        public static bool IsPair(List<int> list)
        {
            return list.Count == 2;
        }

        /// <summary>
        /// Checks if the tile is a man
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a man</returns>   
        public static bool IsMan(int tile)
        {
            return tile <= 8;
        }

        /// <summary>
        /// Checks if the tile is a pin
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a pin</returns>   
        public static bool IsPin(int tile)
        {
            return 8 < tile && tile <= 17;
        }

        /// <summary>
        /// Checks if the tile is a pin
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a pin</returns>   
        public static bool IsSou(int tile)
        {
            return 17 < tile && tile <= 26;
        }

        /// <summary>
        /// Checks if the tile is a honor
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a honor</returns>   
        public static bool IsHonor(int tile)
        {
            return tile >= 27;
        }

        /// <summary>
        /// Checks if the tile is a dragon
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a dragon</returns>   
        public static bool IsDragon(int tile_34)
        {
            return tile_34 >= 31;
        }

        /// <summary>
        /// Checks if the tile is a terminal
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a terminal</returns>   
        public static bool IsTerminal(int tile)
        {
            return C.TERMINAL_INDICES.Contains(tile);
        }

        /// <summary>
        /// Checks if the tile is a terminal dora indicator
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>True if the tile is a terminal dora indicator</returns>   
        public static bool IsDoraIndicatorForTerminal(int tile)
        {
            return tile == 7 || tile == 8 || tile == 16 || tile == 17 || tile == 25 || tile == 26;
        }


        /// <summary>
        /// Checks if the group contains a terminal
        /// </summary>
        /// <param name="handSet"> The group, in 34 format</param>
        /// <returns>True if the group contains a terminal</returns>   
        public static bool ContainsTerminal(List<int> handSet)
        {
            return (from x in handSet
                        select C.TERMINAL_INDICES.Contains(x)).ToList().Any();
        }

        /// <summary>
        /// Return the tile value in 0-8 format
        /// </summary>
        /// <param name="tile"> The tile, in 34 format</param>
        /// <returns>The tile value in 0-8 format</returns>   
        public static int Simplify(int tile)
        {
            return tile - 9 * (tile / 9);
        }

        /// <summary>
        /// Find indices that do not have a tile for them or their neighbors
        /// </summary>
        /// <param name="hand34"> The hand, in 34-count format</param>
        /// <returns>Array of isolated tiles indices</returns>   
        public static Stack<int> FindIsolatedTileIndices(int[] hand34)
        {
            var isolatedIndices = new Stack<int>();
            foreach (var x in Enumerable.Range(0, C.CHUN + 1 - 0))
            {
                // for honor tiles we don't need to check nearby tiles
                if (IsHonor(x) && hand34[x] == 0)
                {
                    isolatedIndices.Push(x);
                }
                else
                {
                    var simplified = Simplify(x);
                    // 1 suit tile
                    if (simplified == 0)
                    {
                        if (hand34[x] == 0 && hand34[x + 1] == 0)
                        {
                            isolatedIndices.Push(x);
                        }
                    }
                    else if (simplified == 8)
                    {
                        // 9 suit tile
                        if (hand34[x] == 0 && hand34[x - 1] == 0)
                        {
                            isolatedIndices.Push(x);
                        }
                    }
                    else
                    {
                        // 2-8 tiles tiles
                        if (hand34[x] == 0 && hand34[x - 1] == 0 && hand34[x + 1] == 0)
                        {
                            isolatedIndices.Push(x);
                        }
                    }
                }
            }
            return isolatedIndices;
        }


        /// <summary>
        /// Find indices that do not have a tile for them or their +1 or +2 neighbors
        /// </summary>
        /// <param name="hand34"> The hand, in 34-count format</param>
        /// <returns>Array of strictly isolated tiles indices</returns>   
        public static bool IsTileStrictlyIsolated(List<int> hand34, int tile34)
        {
            if (IsHonor(tile34))
            {
                return hand34[tile34] - 1 <= 0;
            }
            var simplified = Simplify(tile34);
            List<int> indices = new List<int>();
            // 1 suit tile
            if (simplified == 0)
            {
                indices = new List<int> {
                    tile34,
                    tile34 + 1,
                    tile34 + 2
                };
            }
            else if (simplified == 1)
            {
                // 2 suit tile
                indices = new List<int> {
                    tile34 - 1,
                    tile34,
                    tile34 + 1,
                    tile34 + 2
                };
            }
            else if (simplified == 7)
            {
                // 8 suit tile
                indices = new List<int> {
                    tile34 - 2,
                    tile34 - 1,
                    tile34,
                    tile34 + 1
                };
            }
            else if (simplified == 8)
            {
                // 9 suit tile
                indices = new List<int> {
                    tile34 - 2,
                    tile34 - 1,
                    tile34
                };
            }
            else
            {
                // 3-7 tiles tiles
                indices = new List<int> {
                    tile34 - 2,
                    tile34 - 1,
                    tile34,
                    tile34 + 1,
                    tile34 + 2
                };
            }
            var isolated = true;
            foreach (var idx in indices)
            {
                // we don't want to count our tile as it is in hand already
                if (idx == tile34)
                {
                    isolated |= hand34[idx] - 1 <= 0;
                }
                else
                {
                    isolated |= hand34[idx] == 0;
                }
            }
            return isolated;
        }

        /// <summary>
        /// Separate tiles by suits and count them
        /// </summary>
        /// <param name="hand34"> The hand, in 34-count format</param>
        /// <returns>The count of tiles by suit</returns>   
        public static Dictionary<string, int> CountTilesBySuit(List<int> tiles34)
        {
            var suits = new Dictionary<string, int>();
            suits.Add("sou", 0);
            suits.Add("man", 0);
            suits.Add("pin", 0);
            suits.Add("honor", 0);

            foreach (var x in Enumerable.Range(0, 34 - 0))
            {
                var nbTiles = tiles34[x];
                if (nbTiles == 0)
                {
                    continue;
                }
                suits["sou"] += (IsSou(x) ? nbTiles : 0);
                suits["man"] += (IsMan(x) ? nbTiles : 0);
                suits["pin"] += (IsPin(x) ? nbTiles : 0);
                suits["honor"] += (IsHonor(x) ? nbTiles : 0);
            }
            return suits;
        }

        /// <summary>
        /// Get the suit order the hand is in
        /// </summary>
        /// <param name="str"> The string representation fo the hand</param>
        /// <returns>a string of the suits, ordered by appearance in the hand</returns>   
        public static string GetSuitOrder(string str)
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

        /// <summary>
        /// Checks if all tiles are in the given list of values
        /// </summary>
        /// <param name="tiles"> The list of the tiles, in any format</param>
        /// <param name="values"> The list to check the tiles against, in the same format</param>
        /// <returns>a string of the suits, ordered by appearance in the hand</returns>   
        public static bool AreAllTilesInIndices(List<int> tiles, IEnumerable<int> values)
        {
            foreach (var tile in tiles)
            {
                if (!values.Contains(tile))
                {
                    return false;
                }
            }
            return true;
        }

    }
}