//=====================================================
//=== Code shamelessly converted from   ==============
//= https://github.com/MahjongRepository/mahjong  ===
//====  All credits go to her/him         ============
//=====================================================
using System.Collections.Generic;
using System.Linq;
using System;
using C = kandora.bot.mahjong.Constants;

namespace kandora.bot.mahjong
{
    public class ShantenCalculator
    {

        int[] tiles = new int[34];

        int number_melds = 0;

        int number_tatsu = 0;

        int number_pairs = 0;

        int number_jidahai = 0;

        int number_characters = 0;

        int number_isolated_tiles = 0;

        int min_shanten = 0;

        // 
        //         Return the minimum shanten for provided hand,
        //         it will consider chiitoitsu and kokushi options if possible.
        //         
        public int Calculate_shanten(int[] tiles_34, bool use_chiitoitsu = true, bool use_kokushi = true)
        {
            var shanten_results = new List<int> {
                this.calculate_shanten_for_regular_hand(tiles_34)
            };
            if (use_chiitoitsu)
            {
                shanten_results.Add(this.calculate_shanten_for_chiitoitsu_hand(tiles_34));
            }
            if (use_kokushi)
            {
                shanten_results.Add(this.calculate_shanten_for_kokushi_hand(tiles_34));
            }
            return shanten_results.Min();
        }

        // 
        //         Calculate the number of shanten for chiitoitsu hand
        //         
        private int calculate_shanten_for_chiitoitsu_hand(int[] tiles_34)
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
        private int calculate_shanten_for_kokushi_hand(int[] tiles_34)
        {
            var indices = C.TERMINAL_INDICES.Concat(C.HONOR_INDICES);
            var completed_terminals = 0;
            foreach (var i in indices)
            {
                completed_terminals += ((tiles_34[i]) >= 2) ? 1: 0;
            }
            var terminals = 0;
            foreach (var i in indices)
            {
                terminals += (tiles_34[i] != 0) ? 1 : 0;
            }
            return 13 - terminals - ((completed_terminals > 0) ? 1 : 0);
        }

        // 
        //         Calculate the number of shanten for regular hand
        //         
        private int calculate_shanten_for_regular_hand(int[] input)
        {
            // we will modify tiles array later, so we need to use a copy
            var tiles_34 = new int[34];
            input.CopyTo(tiles_34, 0);
            this._init(tiles_34);
            var count_of_tiles = tiles_34.Sum();
            if(count_of_tiles > 14)
            {
                throw new Exception($"Too many tiles = {count_of_tiles}");
            }
            this._remove_character_tiles(count_of_tiles);
            var init_mentsu = (int)Math.Floor((decimal)((14 - count_of_tiles) / 3));
            this._scan(init_mentsu);
            return this.min_shanten;
        }

        private void _init(int[] tiles)
        {
            this.tiles = tiles;
            this.number_melds = 0;
            this.number_tatsu = 0;
            this.number_pairs = 0;
            this.number_jidahai = 0;
            this.number_characters = 0;
            this.number_isolated_tiles = 0;
            this.min_shanten = 8;
        }

        private void _scan(int init_mentsu)
        {
            this.number_characters = 0;
            foreach (var i in Enumerable.Range(0, 27 - 0))
            {
                this.number_characters |= ((this.tiles[i] == 4) ? 1 : 0) << i;
            }
            this.number_melds += init_mentsu;
            this._run(0);
        }

        private void _run(int depth)
        {
            if (this.min_shanten == C.AGARI_STATE)
            {
                return;
            }
            while (this.tiles[depth] == 0)
            {
                depth += 1;
                if (depth >= 27)
                {
                    break;
                }
            }
            if (depth >= 27)
            {
                this._update_result();
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
            if (this.tiles[depth] == 4)
            {
                this._increase_set(depth);
                if (i < 7 && this.tiles[depth + 2] > 0)
                {
                    if (this.tiles[depth + 1] > 0)
                    {
                        this._increase_syuntsu(depth);
                        this._run(depth + 1);
                        this._decrease_syuntsu(depth);
                    }
                    this._increase_tatsu_second(depth);
                    this._run(depth + 1);
                    this._decrease_tatsu_second(depth);
                }
                if (i < 8 && this.tiles[depth + 1] > 0)
                {
                    this._increase_tatsu_first(depth);
                    this._run(depth + 1);
                    this._decrease_tatsu_first(depth);
                }
                this._increase_isolated_tile(depth);
                this._run(depth + 1);
                this._decrease_isolated_tile(depth);
                this._decrease_set(depth);
                this._increase_pair(depth);
                if (i < 7 && this.tiles[depth + 2] > 0)
                {
                    if (this.tiles[depth + 1] > 0)
                    {
                        this._increase_syuntsu(depth);
                        this._run(depth);
                        this._decrease_syuntsu(depth);
                    }
                    this._increase_tatsu_second(depth);
                    this._run(depth + 1);
                    this._decrease_tatsu_second(depth);
                }
                if (i < 8 && this.tiles[depth + 1] > 0)
                {
                    this._increase_tatsu_first(depth);
                    this._run(depth + 1);
                    this._decrease_tatsu_first(depth);
                }
                this._decrease_pair(depth);
            }
            if (this.tiles[depth] == 3)
            {
                this._increase_set(depth);
                this._run(depth + 1);
                this._decrease_set(depth);
                this._increase_pair(depth);
                if (i < 7 && this.tiles[depth + 1] > 0 && this.tiles[depth + 2] > 0)
                {
                    this._increase_syuntsu(depth);
                    this._run(depth + 1);
                    this._decrease_syuntsu(depth);
                }
                else
                {
                    if (i < 7 && this.tiles[depth + 2] > 0)
                    {
                        this._increase_tatsu_second(depth);
                        this._run(depth + 1);
                        this._decrease_tatsu_second(depth);
                    }
                    if (i < 8 && this.tiles[depth + 1] > 0)
                    {
                        this._increase_tatsu_first(depth);
                        this._run(depth + 1);
                        this._decrease_tatsu_first(depth);
                    }
                }
                this._decrease_pair(depth);
                if (i < 7 && this.tiles[depth + 2] >= 2 && this.tiles[depth + 1] >= 2)
                {
                    this._increase_syuntsu(depth);
                    this._increase_syuntsu(depth);
                    this._run(depth);
                    this._decrease_syuntsu(depth);
                    this._decrease_syuntsu(depth);
                }
            }
            if (this.tiles[depth] == 2)
            {
                this._increase_pair(depth);
                this._run(depth + 1);
                this._decrease_pair(depth);
                if (i < 7 && this.tiles[depth + 2] > 0 && this.tiles[depth + 1] > 0)
                {
                    this._increase_syuntsu(depth);
                    this._run(depth);
                    this._decrease_syuntsu(depth);
                }
            }
            if (this.tiles[depth] == 1)
            {
                if (i < 6 && this.tiles[depth + 1] == 1 && this.tiles[depth + 2] > 0 && this.tiles[depth + 3] != 4)
                {
                    this._increase_syuntsu(depth);
                    this._run(depth + 2);
                    this._decrease_syuntsu(depth);
                }
                else
                {
                    this._increase_isolated_tile(depth);
                    this._run(depth + 1);
                    this._decrease_isolated_tile(depth);
                    if (i < 7 && this.tiles[depth + 2] > 0)
                    {
                        if (this.tiles[depth + 1] > 0)
                        {
                            this._increase_syuntsu(depth);
                            this._run(depth + 1);
                            this._decrease_syuntsu(depth);
                        }
                        this._increase_tatsu_second(depth);
                        this._run(depth + 1);
                        this._decrease_tatsu_second(depth);
                    }
                    if (i < 8 && this.tiles[depth + 1] > 0)
                    {
                        this._increase_tatsu_first(depth);
                        this._run(depth + 1);
                        this._decrease_tatsu_first(depth);
                    }
                }
            }
        }

        private void _update_result()
        {
            var ret_shanten = 8 - this.number_melds * 2 - this.number_tatsu - this.number_pairs;
            var n_mentsu_kouho = this.number_melds + this.number_tatsu;
            if (this.number_pairs>0)
            {
                n_mentsu_kouho += this.number_pairs - 1;
            }
            else if (this.number_characters > 0 && this.number_isolated_tiles > 0)
            {
                if ((this.number_characters | this.number_isolated_tiles) == this.number_characters)
                {
                    ret_shanten += 1;
                }
            }
            if (n_mentsu_kouho > 4)
            {
                ret_shanten += n_mentsu_kouho - 4;
            }
            if (ret_shanten != C.AGARI_STATE && ret_shanten < this.number_jidahai)
            {
                ret_shanten = this.number_jidahai;
            }
            if (ret_shanten < this.min_shanten)
            {
                this.min_shanten = ret_shanten;
            }
        }

        void _increase_set(int k)
        {
            this.tiles[k] -= 3;
            this.number_melds += 1;
        }

        void _decrease_set(int k)
        {
            this.tiles[k] += 3;
            this.number_melds -= 1;
        }

        void _increase_pair(int k)
        {
            this.tiles[k] -= 2;
            this.number_pairs += 1;
        }

        void _decrease_pair(int k)
        {
            this.tiles[k] += 2;
            this.number_pairs -= 1;
        }

        void _increase_syuntsu(int k)
        {
            this.tiles[k] -= 1;
            this.tiles[k + 1] -= 1;
            this.tiles[k + 2] -= 1;
            this.number_melds += 1;
        }

         void _decrease_syuntsu(int k)
        {
            this.tiles[k] += 1;
            this.tiles[k + 1] += 1;
            this.tiles[k + 2] += 1;
            this.number_melds -= 1;
        }

         void _increase_tatsu_first(int k)
        {
            this.tiles[k] -= 1;
            this.tiles[k + 1] -= 1;
            this.number_tatsu += 1;
        }

         void _decrease_tatsu_first(int k)
        {
            this.tiles[k] += 1;
            this.tiles[k + 1] += 1;
            this.number_tatsu -= 1;
        }

         void _increase_tatsu_second(int k)
        {
            this.tiles[k] -= 1;
            this.tiles[k + 2] -= 1;
            this.number_tatsu += 1;
        }

         void _decrease_tatsu_second(int k)
        {
            this.tiles[k] += 1;
            this.tiles[k + 2] += 1;
            this.number_tatsu -= 1;
        }

         void _increase_isolated_tile(int k)
        {
            this.tiles[k] -= 1;
            this.number_isolated_tiles |= 1 << k;
        }

         void _decrease_isolated_tile(int k)
        {
            this.tiles[k] += 1;
            this.number_isolated_tiles |= 1 << k;
        }

         int _scan_chiitoitsu_and_kokushi(bool chiitoitsu, bool kokushi)
        {
            int ret_shanten;
            var shanten = this.min_shanten;
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
            var completed_terminals = 0;
            foreach (var i in indices)
            {
                completed_terminals += (this.tiles[i] >= 2) ? 1 : 0;
            }
            var terminals = 0;
            foreach (var i in indices)
            {
                terminals += (this.tiles[i] != 0) ? 1 : 0;
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
            var completed_pairs = completed_terminals;
            foreach (var i in indices)
            {
                completed_pairs += (this.tiles[i] >= 2) ? 1 : 0;
            }
            var pairs = terminals;
            foreach (var i in indices)
            {
                pairs += (this.tiles[i] != 0) ? 1 : 0;
            }
            if (chiitoitsu)
            {
                ret_shanten = 6 - completed_pairs + ((pairs < 7) ? 7 - pairs : 0);
                if (ret_shanten < shanten)
                {
                    shanten = ret_shanten;
                }
            }
            if (kokushi)
            {
                ret_shanten = 13 - terminals - ((completed_terminals>0) ? 1 : 0);
                if (ret_shanten < shanten)
                {
                    shanten = ret_shanten;
                }
            }
            return shanten;
        }

         void _remove_character_tiles(int nc)
        {
            var number = 0;
            var isolated = 0;
            foreach (var i in Enumerable.Range(27, 34 - 27))
            {
                if (this.tiles[i] == 4)
                {
                    this.number_melds += 1;
                    this.number_jidahai += 1;
                    number |= 1 << i - 27;
                    isolated |= 1 << i - 27;
                }
                if (this.tiles[i] == 3)
                {
                    this.number_melds += 1;
                }
                if (this.tiles[i] == 2)
                {
                    this.number_pairs += 1;
                }
                if (this.tiles[i] == 1)
                {
                    isolated |= 1 << i - 27;
                }
            }
            if (this.number_jidahai>0 && nc % 3 == 2)
            {
                this.number_jidahai -= 1;
            }
            if (isolated>0)
            {
                this.number_isolated_tiles |= 1 << 27;
                if ((number | isolated) == number)
                {
                    this.number_characters |= 1 << 27;
                }
            }
        }
    }
}