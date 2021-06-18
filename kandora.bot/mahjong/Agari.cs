namespace kandora.bot.mahjong
{
    using System.Linq;
    using System.Collections.Generic;

    public class Agari
    {
        /// <summary>
        /// Determine was it win or not
        /// </summary>
        /// <param name="tiles34">34count format hand (</param>
        /// <param name="openSets34">The 34idx list of the melds</param>
        /// <returns>True if the hand is winning</returns>
        public static bool IsAgari(int[] tiles34, List<List<int>> openSets34 = null)
        {
            // we will modify them later, so we need to use a copy
            var tiles = new int[34];
            tiles34.CopyTo(tiles,0);
            // With open hand we need to remove open sets from hand and replace them with isolated pon sets
            // it will allow to determine agari state correctly
            if (openSets34 != null && openSets34.Count > 0)
            {
                var isolatedTiles = new Stack<int>(Utils.FindIsolatedTileIndices(tiles));
                foreach (var meld in openSets34)
                {
                    if (isolatedTiles == null || isolatedTiles.Count == 0)
                    {
                        break;
                    }
                    var isolatedTile = isolatedTiles.Pop();
                    tiles[meld[0]] -= 1;
                    tiles[meld[1]] -= 1;
                    tiles[meld[2]] -= 1;
                    // kan
                    if (meld.Count > 3)
                    {
                        tiles[meld[3]] -= 1;
                    }
                    tiles[isolatedTile] = 3;
                }
            }
            int j = 1 << tiles[27] | 1 << tiles[28] | 1 << tiles[29] | 1 << tiles[30] | 1 << tiles[31] | 1 << tiles[32] | 1 << tiles[33];
            if (j >= 0x10)
            {
                return false;
            }
            // 13 orphans
            if ((j & 3) == 2 && tiles[0] * tiles[8] * tiles[9] * tiles[17] * tiles[18] * tiles[26] * tiles[27] * tiles[28] * tiles[29] * tiles[30] * tiles[31] * tiles[32] * tiles[33] == 2)
            {
                return true;
            }
            // seven pairs
            if ((j & 10) == 0 && (from i in Enumerable.Range(0, 34 - 0)
                                select (tiles[i] == 2)? 1 : 0).ToList().Sum() == 7)
            {
                return true;
            }
            if ((j & 2) != 0)
            {
                return false;
            }
            var n00 = tiles[0] + tiles[3] + tiles[6];
            var n01 = tiles[1] + tiles[4] + tiles[7];
            var n02 = tiles[2] + tiles[5] + tiles[8];
            var n10 = tiles[9] + tiles[12] + tiles[15];
            var n11 = tiles[10] + tiles[13] + tiles[16];
            var n12 = tiles[11] + tiles[14] + tiles[17];
            var n20 = tiles[18] + tiles[21] + tiles[24];
            var n21 = tiles[19] + tiles[22] + tiles[25];
            var n22 = tiles[20] + tiles[23] + tiles[26];
            var n0 = (n00 + n01 + n02) % 3;
            if (n0 == 1)
            {
                return false;
            }
            var n1 = (n10 + n11 + n12) % 3;
            if (n1 == 1)
            {
                return false;
            }
            var n2 = (n20 + n21 + n22) % 3;
            if (n2 == 1)
            {
                return false;
            }
            //seriously what
            var wtf = (n0 == 2 ? 1 : 0) 
                + (n1 == 2 ? 1 : 0)
                + (n2 == 2 ? 1 : 0)
                + (tiles[27] == 2 ? 1 : 0)
                + (tiles[28] == 2 ? 1 : 0)
                + (tiles[29] == 2 ? 1 : 0)
                + (tiles[30] == 2 ? 1 : 0)
                + (tiles[31] == 2 ? 1 : 0)
                + (tiles[32] == 2 ? 1 : 0)
                + (tiles[33] == 2 ? 1 : 0);
            if (wtf != 1)
            {
                return false;
            }
            //wizardry incoming
            var nn0 = (n00 * 1 + n01 * 2) % 3;
            var m0 = toMeld(tiles, 0);
            var nn1 = (n10 * 1 + n11 * 2) % 3;
            var m1 = toMeld(tiles, 9);
            var nn2 = (n20 * 1 + n21 * 2) % 3;
            var m2 = toMeld(tiles, 18);
            if ((j & 4)!= 0)
            {
                return (n0 | nn0 | n1 | nn1 | n2 | nn2) == 0 && isMentsu(m0) && isMentsu(m1) && isMentsu(m2);
            }
            if (n0 == 2)
            {
                return (n1 | nn1 | n2 | nn2) == 0 && isMentsu(m1) && isMentsu(m2) && isAtamaMentsu(nn0, m0);
            }
            if (n1 == 2)
            {
                return (n2 | nn2 | n0 | nn0) == 0 && isMentsu(m2) && isMentsu(m0) && isAtamaMentsu(nn1, m1);
            }
            if (n2 == 2)
            {
                return (n0 | nn0 | n1 | nn1) == 0 && isMentsu(m0) && isMentsu(m1) && isAtamaMentsu(nn2, m2);
            }
            return false;
        }

        private static bool isMentsu(int m)
        {
            var a = m & 7;
            var b = 0;
            var c = 0;
            if (a == 1 || a == 4)
            {
                b = 1;
                c = 1;
            }
            else if (a == 2)
            {
                b = 2;
                c = 2;
            }
            m >>= 3;
            a = (m & 7) - b;
            if (a < 0)
            {
                return false;
            }
            var isNotMentsu = false;
            foreach (var _ in Enumerable.Range(0, 6 - 0))
            {
                b = c;
                c = 0;
                if (a == 1 || a == 4)
                {
                    b += 1;
                    c += 1;
                }
                else if (a == 2)
                {
                    b += 2;
                    c += 2;
                }
                m >>= 3;
                a = (m & 7) - b;
                if (a < 0)
                {
                    isNotMentsu = true;
                    break;
                }
            }
            if (isNotMentsu)
            {
                return false;
            }
            m >>= 3;
            a = (m & 7) - c;
            return a == 0 || a == 3;
        }

        private static bool isAtamaMentsu(int nn, int m)
        {
            if (nn == 0)
            {
                if ((m & 7 << 6) >= 2 << 6 && isMentsu(m - (2 << 6)))
                {
                    return true;
                }
                if ((m & 7 << 15) >= 2 << 15 && isMentsu(m - (2 << 15)))
                {
                    return true;
                }
                if ((m & 7 << 24) >= 2 << 24 && isMentsu(m - (2 << 24)))
                {
                    return true;
                }
            }
            else if (nn == 1)
            {
                if ((m & 7 << 3) >= 2 << 3 && isMentsu(m - (2 << 3)))
                {
                    return true;
                }
                if ((m & 7 << 12) >= 2 << 12 && isMentsu(m - (2 << 12)))
                {
                    return true;
                }
                if ((m & 7 << 21) >= 2 << 21 && isMentsu(m - (2 << 21)))
                {
                    return true;
                }
            }
            else if (nn == 2)
            {
                if ((m & 7 << 0) >= 2 << 0 && isMentsu(m - (2 << 0)))
                {
                    return true;
                }
                if ((m & 7 << 9) >= 2 << 9 && isMentsu(m - (2 << 9)))
                {
                    return true;
                }
                if ((m & 7 << 18) >= 2 << 18 && isMentsu(m - (2 << 18)))
                {
                    return true;
                }
            }
            return false;
        }

        private static int toMeld(int[] tiles, int d)
        {
            var result = 0;
            foreach (var i in Enumerable.Range(0, 9 - 0))
            {
                result |= tiles[d + i] << i * 3;
            }
            return result;
        }
    }
}
