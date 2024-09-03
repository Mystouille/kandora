using System.Collections.Generic;
using System.Linq;
using System;
using C = kandora.bot.mahjong.Constants;
using kandora.bot.resources;
using kandora.bot.mahjong.handcalc.yaku.yakuman;
using kandora.bot.utils;
using System.Text;
using DSharpPlus.SlashCommands;
using DSharpPlus;

namespace kandora.bot.mahjong
{
    public class ShantenCalculator
    {

        int[] tiles = new int[34];

        int nbMelds = 0;

        int nbTatsu = 0;

        int nbPairs = 0;

        int nbJidahai = 0;

        int nbCharacters = 0;

        int nbIsolatedTiles = 0;

        int minShanten = 0;


        public string getUkeireDisplayForHand(string hand, string doras, DiscordClient client, string ukeireDisplay = "Yes", string discards = "")
        {

            var sb = new StringBuilder();
            var ukeires = this.getUkeire(hand, doras, discards);
            var oldShanten = -2;

            var shanten = this.GetNbShanten(TilesConverter.FromStringTo34Count(hand));
            if(shanten == -1)
            {
                sb.AppendLine("Tsumo?");
            }
            sb.Append($"Ukeire:");
            foreach (var discard in ukeires)
            {
                var tileDiscarded = discard.TileDiscarded;
                var discardStr = TilesConverter.From34IdxTileToString(tileDiscarded);
                var discardEmoji = HandParser.GetHandEmojiString(discardStr, client);
                var nbUkeire = discard.NbAcceptedTiles;
                var ukeireListStr = TilesConverter.From34CountHandToString(discard.DrawnTileData.ToList().Select(x=>x.IsAccepted?1:0).ToList());
                var ukeireListEmoji = HandParser.GetHandEmojiString(ukeireListStr, client);
                if (oldShanten != discard.Shanten)
                {
                    var shantenStr = this.GetShantenStr(discard.Shanten);
                    sb.AppendLine($"\n- {shantenStr}:");
                    oldShanten = discard.Shanten;
                }
                if (ukeireDisplay == "Full")
                {
                    var ukeireSb = new StringBuilder();
                    for (var tileDrawn = 0; tileDrawn < discard.DrawnTileData.Length; tileDrawn++)
                    {
                        if (!discard.DrawnTileData[tileDrawn].IsAccepted)
                        {
                            continue;
                        }
                        var ukeireStr = TilesConverter.From34IdxTileToString(tileDrawn);
                        var ukeireEmoji = HandParser.GetHandEmojiString(ukeireStr, client);
                        var leadToGoodTenpai = discard.DrawnTileData[tileDrawn].LeadToGoodTenpai;
                        ukeireSb.Append($"{ukeireEmoji}{(leadToGoodTenpai ? "\\*" : "")}");
                    }
                    sb.AppendLine($"{discardEmoji}:\t{nbUkeire}({discard.NbAcceptedTilesForGoodTenpai}\\*) [{ukeireSb.ToString()}]");
                }
                else
                {
                    sb.Append($"{discardEmoji}x{nbUkeire} ");
                }
            }
            return sb.ToString();
        }

        // return the shanten of a hand and the ukeire for each discard (except those increasing the shanten)
        public List<AcceptanceData> getUkeire(string handStr, string doras = "", string forcedTiles = "")
        {
            var tiles_34 = TilesConverter.FromStringTo34Count(handStr, has_aka_dora: true).ToList();
            var indicators_34 = TilesConverter.FromStringDoraTo34CountIndicator(doras).ToList();
            var forcedTiles_34 = TilesConverter.FromStringTo34Count(forcedTiles, has_aka_dora: true).ToList();
            return getUkeire(tiles_34, indicators_34, forcedTiles_34);
        }

        public List<AcceptanceData> getUkeire(List<int> tiles_34, List<int> indicators_34, List<int> forcedTiles_34)
        {
            int minShanten = 8;
            var potentialDiscards = new List<AcceptanceData>();
            var hand = tiles_34.ToList();

            //get min shanten for each discard and init return struct
            for (var i = 0; i < tiles_34.Count; i++)
            {
                if (hand[i] == 0)
                {
                    continue;
                }
                hand[i]--;
                var shanten = GetNbShanten(hand.ToArray());
                if (shanten == minShanten)
                {
                    potentialDiscards.Add(new AcceptanceData(i, shanten));
                }
                else if (shanten < minShanten)
                {
                    minShanten = shanten;
                    potentialDiscards.Clear();
                    potentialDiscards.Add(new AcceptanceData(i, shanten));
                }
                hand[i]++;
            }

            //if ukeire display is forced, fix the return struct
            if (forcedTiles_34.Count > 0)
            {
                potentialDiscards.Clear();
                for (var i = 0; i < forcedTiles_34.Count; i++)
                {
                    if (hand[i] == 0 || forcedTiles_34[i] == 0)
                    {
                        continue;
                    }
                    hand[i]--;
                    var shanten = GetNbShanten(hand.ToArray());
                    potentialDiscards.Add(new AcceptanceData(i, shanten));
                    hand[i]++;
                }
            }

            for (int i = 0; i<potentialDiscards.Count; i++)
            {
                var discardData = potentialDiscards[i];
                var tileDiscarded = discardData.TileDiscarded;
                hand[tileDiscarded]--;
                for (var tileDrawn = 0; tileDrawn<34; tileDrawn++)
                {
                    var potentialUkeire = 4 - hand[tileDrawn] - indicators_34[tileDrawn];
                    if (potentialUkeire <= 0)
                    {
                        discardData.DrawnTileData[tileDrawn] = new DrawnTileInfo(false, false);
                        continue;
                    }
                    hand[tileDrawn]++;
                    var shantenAfterDraw = GetNbShanten(hand.ToArray());
                    // iterate only over the draws that improve the hand
                    if (shantenAfterDraw < discardData.Shanten)
                    {
                        var isGoodTenpai = false;
                        //add additional "good tenpai info" if it's a tenpai
                        if(shantenAfterDraw == 0)
                        {
                            var tenpaiUkeire = getUkeire(hand, indicators_34, new List<int>());
                            isGoodTenpai = tenpaiUkeire.Where(acceptance => acceptance.NbAcceptedTiles >= 6).Count() > 0;
                        }
                        discardData.AddNbAcceptedTiles(potentialUkeire);
                        if (isGoodTenpai)
                        {
                            discardData.AddNbAcceptedTilesForGoodTenpai(potentialUkeire);
                        }
                        discardData.DrawnTileData[tileDrawn] = new DrawnTileInfo(true, isGoodTenpai);
                    }
                    else
                    {
                        discardData.DrawnTileData[tileDrawn] = new DrawnTileInfo(false, false);
                    }
                    hand[tileDrawn]--;
                }
                hand[tileDiscarded]++;
            }
            //sort by shanten and ukeire
            var sortedList = potentialDiscards.ToList().OrderByDescending(x => 9000 - x.Shanten*1000 + x.NbAcceptedTiles).ToList();

            return sortedList;
        }

        // 
        //         Return the minimum shanten for provided hand,
        //         it will consider chiitoitsu and kokushi options if possible.
        //         
        public int GetNbShanten(int[] tiles_34, bool useChiitoitsu = true, bool use_kokushi = true)
        {
            var shantenResults = new List<int> {
                GetShantenForRegularHand(tiles_34)
            };
            if (useChiitoitsu)
            {
                shantenResults.Add(GetShantenForChiitoi(tiles_34));
            }
            if (use_kokushi)
            {
                shantenResults.Add(GetShantenForKokushi(tiles_34));
            }
            return shantenResults.Min();
        }

        public string GetNbShantenStr(int[] tiles_34, bool useChiitoitsu = true, bool use_kokushi = true)
        {
            return GetShantenStr(GetNbShanten(tiles_34, useChiitoitsu, use_kokushi));
        }

        // 
        //         Return a pretty text for a given shanten
        //    
        public string GetShantenStr(int shanten)
        {
            if (shanten < -1)
            {
                return "";
            }
            if (shanten == -1)
            {
                return "TSUMO";
            }
            if (shanten == 0)
            {
                return "tenpai";
            }
            //if (shanten == 1)
            //{
            //    return "ii-shanten";
            //}
            //if (shanten == 2)
            //{
            //    return "ryan-shanten";
            //}
            return $"{shanten}-shanten";
        }

        // 
        //         Calculate the number of shanten for chiitoitsu hand
        //         
        private int GetShantenForChiitoi(int[] tiles_34)
        {
            var pairs = (from x in tiles_34
                            where x >= 2
                            select x).Count();
            if (pairs == 7)
            {
                return C.AGARI_STATE;
            }
            return 6 - pairs;
        }

        // 
        //         Calculate the number of shanten for kokushi musou hand
        //         
        private int GetShantenForKokushi(int[] tiles_34)
        {
            var indices = C.TERMINAL_INDICES.Concat(C.HONOR_INDICES);
            var completedTerminal = 0;
            foreach (var i in indices)
            {
                completedTerminal += ((tiles_34[i]) >= 2) ? 1: 0;
            }
            var terminals = 0;
            foreach (var i in indices)
            {
                terminals += (tiles_34[i] != 0) ? 1 : 0;
            }
            return 13 - terminals - ((completedTerminal > 0) ? 1 : 0);
        }

        // 
        //         Calculate the number of shanten for regular hand
        //         
        private int GetShantenForRegularHand(int[] input)
        {
            // we will modify tiles array later, so we need to use a copy
            var tiles34 = new int[34];
            input.CopyTo(tiles34, 0);
            _init(tiles34);
            var count_of_tiles = tiles34.Sum();
            if(count_of_tiles > 14)
            {
                throw new Exception(String.Format(Resources.commandError_TooManyTiles,count_of_tiles));
            }
            RemoveCharacterTiles(count_of_tiles);
            var init_mentsu = (int)Math.Floor((decimal)((14 - count_of_tiles) / 3));
            Scan(init_mentsu);
            return minShanten;
        }

        private void _init(int[] tiles)
        {
            this.tiles = tiles;
            nbMelds = 0;
            nbTatsu = 0;
            nbPairs = 0;
            nbJidahai = 0;
            nbCharacters = 0;
            nbIsolatedTiles = 0;
            minShanten = 8;
        }

        private void Scan(int init_mentsu)
        {
            nbCharacters = 0;
            foreach (var i in Enumerable.Range(0, 27 - 0))
            {
                nbCharacters |= ((tiles[i] == 4) ? 1 : 0) << i;
            }
            nbMelds += init_mentsu;
            run(0);
        }

        private void run(int depth)
        {
            if (minShanten == C.AGARI_STATE)
            {
                return;
            }
            while (tiles[depth] == 0)
            {
                depth += 1;
                if (depth >= 27)
                {
                    break;
                }
            }
            if (depth >= 27)
            {
                _update_result();
                return;
            }
            var i = depth;
            if (i > 8)
            {
                i -= 9;
            }
            if (i > 8)
            {
                i -= 9;
            }
            if (tiles[depth] == 4)
            {
                increaseSet(depth);
                if (i < 7 && tiles[depth + 2] > 0)
                {
                    if (tiles[depth + 1] > 0)
                    {
                        increaseShuntsu(depth);
                        run(depth + 1);
                        decreaseShuntsu(depth);
                    }
                    increaseTatsuSecond(depth);
                    run(depth + 1);
                    decreaseTatsuSecond(depth);
                }
                if (i < 8 && tiles[depth + 1] > 0)
                {
                    increaseTatsuFirst(depth);
                    run(depth + 1);
                    decreaseTatsuFirst(depth);
                }
                increaseIsolatedTile(depth);
                run(depth + 1);
                decreaseIsolatedTile(depth);
                decreaseSet(depth);
                increasePair(depth);
                if (i < 7 && tiles[depth + 2] > 0)
                {
                    if (tiles[depth + 1] > 0)
                    {
                        increaseShuntsu(depth);
                        run(depth);
                        decreaseShuntsu(depth);
                    }
                    increaseTatsuSecond(depth);
                    run(depth + 1);
                    decreaseTatsuSecond(depth);
                }
                if (i < 8 && tiles[depth + 1] > 0)
                {
                    increaseTatsuFirst(depth);
                    run(depth + 1);
                    decreaseTatsuFirst(depth);
                }
                decreasePair(depth);
            }
            if (tiles[depth] == 3)
            {
                increaseSet(depth);
                run(depth + 1);
                decreaseSet(depth);
                increasePair(depth);
                if (i < 7 && tiles[depth + 1] > 0 && tiles[depth + 2] > 0)
                {
                    increaseShuntsu(depth);
                    run(depth + 1);
                    decreaseShuntsu(depth);
                }
                else
                {
                    if (i < 7 && tiles[depth + 2] > 0)
                    {
                        increaseTatsuSecond(depth);
                        run(depth + 1);
                        decreaseTatsuSecond(depth);
                    }
                    if (i < 8 && tiles[depth + 1] > 0)
                    {
                        increaseTatsuFirst(depth);
                        run(depth + 1);
                        decreaseTatsuFirst(depth);
                    }
                }
                decreasePair(depth);
                if (i < 7 && tiles[depth + 2] >= 2 && tiles[depth + 1] >= 2)
                {
                    increaseShuntsu(depth);
                    increaseShuntsu(depth);
                    run(depth);
                    decreaseShuntsu(depth);
                    decreaseShuntsu(depth);
                }
            }
            if (tiles[depth] == 2)
            {
                increasePair(depth);
                run(depth + 1);
                decreasePair(depth);
                if (i < 7 && tiles[depth + 2] > 0 && tiles[depth + 1] > 0)
                {
                    increaseShuntsu(depth);
                    run(depth);
                    decreaseShuntsu(depth);
                }
            }
            if (tiles[depth] == 1)
            {
                if (i < 6 && tiles[depth + 1] == 1 && tiles[depth + 2] > 0 && tiles[depth + 3] != 4)
                {
                    increaseShuntsu(depth);
                    run(depth + 2);
                    decreaseShuntsu(depth);
                }
                else
                {
                    increaseIsolatedTile(depth);
                    run(depth + 1);
                    decreaseIsolatedTile(depth);
                    if (i < 7 && tiles[depth + 2] > 0)
                    {
                        if (tiles[depth + 1] > 0)
                        {
                            increaseShuntsu(depth);
                            run(depth + 1);
                            decreaseShuntsu(depth);
                        }
                        increaseTatsuSecond(depth);
                        run(depth + 1);
                        decreaseTatsuSecond(depth);
                    }
                    if (i < 8 && tiles[depth + 1] > 0)
                    {
                        increaseTatsuFirst(depth);
                        run(depth + 1);
                        decreaseTatsuFirst(depth);
                    }
                }
            }
        }

        private void _update_result()
        {
            var ret_shanten = 8 - nbMelds * 2 - nbTatsu - nbPairs;
            var n_mentsu_kouho = nbMelds + nbTatsu;
            if (nbPairs>0)
            {
                n_mentsu_kouho += nbPairs - 1;
            }
            else if (nbCharacters > 0 && nbIsolatedTiles > 0)
            {
                if ((nbCharacters | nbIsolatedTiles) == nbCharacters)
                {
                    ret_shanten += 1;
                }
            }
            if (n_mentsu_kouho > 4)
            {
                ret_shanten += n_mentsu_kouho - 4;
            }
            if (ret_shanten != C.AGARI_STATE && ret_shanten < nbJidahai)
            {
                ret_shanten = nbJidahai;
            }
            if (ret_shanten < minShanten)
            {
                minShanten = ret_shanten;
            }
        }

        void increaseSet(int k)
        {
            tiles[k] -= 3;
            nbMelds += 1;
        }

        void decreaseSet(int k)
        {
            tiles[k] += 3;
            nbMelds -= 1;
        }

        void increasePair(int k)
        {
            tiles[k] -= 2;
            nbPairs += 1;
        }

        void decreasePair(int k)
        {
            tiles[k] += 2;
            nbPairs -= 1;
        }

        void increaseShuntsu(int k)
        {
            tiles[k] -= 1;
            tiles[k + 1] -= 1;
            tiles[k + 2] -= 1;
            nbMelds += 1;
        }

         void decreaseShuntsu(int k)
        {
            tiles[k] += 1;
            tiles[k + 1] += 1;
            tiles[k + 2] += 1;
            nbMelds -= 1;
        }

         void increaseTatsuFirst(int k)
        {
            tiles[k] -= 1;
            tiles[k + 1] -= 1;
            nbTatsu += 1;
        }

         void decreaseTatsuFirst(int k)
        {
            tiles[k] += 1;
            tiles[k + 1] += 1;
            nbTatsu -= 1;
        }

         void increaseTatsuSecond(int k)
        {
            tiles[k] -= 1;
            tiles[k + 2] -= 1;
            nbTatsu += 1;
        }

         void decreaseTatsuSecond(int k)
        {
            tiles[k] += 1;
            tiles[k + 2] += 1;
            nbTatsu -= 1;
        }

         void increaseIsolatedTile(int k)
        {
            tiles[k] -= 1;
            nbIsolatedTiles |= 1 << k;
        }

         void decreaseIsolatedTile(int k)
        {
            tiles[k] += 1;
            nbIsolatedTiles |= 1 << k;
        }

         int ScanChiitoiAndKokushi(bool chiitoitsu, bool kokushi)
        {
            int ret_shanten;
            var shanten = minShanten;
            var indices = new int[] {
                0,
                8,
                9,
                17,
                18,
                26,
                27,
                28,
                29,
                30,
                31,
                32,
                33
            };
            var completedTerminals = 0;
            foreach (var i in indices)
            {
                completedTerminals += (tiles[i] >= 2) ? 1 : 0;
            }
            var terminals = 0;
            foreach (var i in indices)
            {
                terminals += (tiles[i] != 0) ? 1 : 0;
            }
            indices = new int[] {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                10,
                11,
                12,
                13,
                14,
                15,
                16,
                19,
                20,
                21,
                22,
                23,
                24,
                25
            };
            var completerPairs = completedTerminals;
            foreach (var i in indices)
            {
                completerPairs += (tiles[i] >= 2) ? 1 : 0;
            }
            var pairs = terminals;
            foreach (var i in indices)
            {
                pairs += (tiles[i] != 0) ? 1 : 0;
            }
            if (chiitoitsu)
            {
                ret_shanten = 6 - completerPairs + ((pairs < 7) ? 7 - pairs : 0);
                if (ret_shanten < shanten)
                {
                    shanten = ret_shanten;
                }
            }
            if (kokushi)
            {
                ret_shanten = 13 - terminals - ((completedTerminals>0) ? 1 : 0);
                if (ret_shanten < shanten)
                {
                    shanten = ret_shanten;
                }
            }
            return shanten;
        }

         void RemoveCharacterTiles(int nc)
        {
            var number = 0;
            var isolated = 0;
            foreach (var i in Enumerable.Range(27, 34 - 27))
            {
                if (tiles[i] == 4)
                {
                    nbMelds += 1;
                    nbJidahai += 1;
                    number |= 1 << i - 27;
                    isolated |= 1 << i - 27;
                }
                if (tiles[i] == 3)
                {
                    nbMelds += 1;
                }
                if (tiles[i] == 2)
                {
                    nbPairs += 1;
                }
                if (tiles[i] == 1)
                {
                    isolated |= 1 << i - 27;
                }
            }
            if (nbJidahai>0 && nc % 3 == 2)
            {
                nbJidahai -= 1;
            }
            if (isolated>0)
            {
                nbIsolatedTiles |= 1 << 27;
                if ((number | isolated) == number)
                {
                    nbCharacters |= 1 << 27;
                }
            }
        }
    }
}