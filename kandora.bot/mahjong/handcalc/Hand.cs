using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C = kandora.bot.mahjong.Constants;
using U = kandora.bot.mahjong.Utils;

namespace kandora.bot.mahjong.handcalc
{

    public class HandCalculator
    {
        public HandConfig config = null;
        HandDivider divider = null;
        public static string ERR_NO_WINNING_TILE = "winning_tile_not_in_hand";
        public static string ERR_OPEN_HAND_RIICHI = "open_hand_riichi_not_allowed";
        public static string ERR_OPEN_HAND_DABURI = "open_hand_daburi_not_allowed";
        public static string ERR_IPPATSU_WITHOUT_RIICHI = "ippatsu_without_riichi_not_allowed";
        public static string ERR_HAND_NOT_WINNING = "hand_not_winning";
        public static string ERR_HAND_NOT_CORRECT = "hand_not_correct";
        public static string ERR_NO_YAKU = "no_yaku";
        public static string ERR_CHANKAN_WITH_TSUMO = "chankan_with_tsumo_not_allowed";
        public static string ERR_RINSHAN_WITHOUT_TSUMO = "rinshan_without_tsumo_not_allowed";
        public static string ERR_HAITEI_WITHOUT_TSUMO = "haitei_without_tsumo_not_allowed";
        public static string ERR_HOUTEI_WITH_TSUMO = "houtei_with_tsumo_not_allowed";
        public static string ERR_HAITEI_WITH_RINSHAN = "haitei_with_rinshan_not_allowed";
        public static string ERR_HOUTEI_WITH_CHANKAN = "houtei_with_chankan_not_allowed";
        public static string ERR_TENHOU_NOT_AS_DEALER = "tenhou_not_as_dealer_not_allowed";
        public static string ERR_TENHOU_WITHOUT_TSUMO = "tenhou_without_tsumo_not_allowed";
        public static string ERR_TENHOU_WITH_MELD = "tenhou_with_meld_not_allowed";
        public static string ERR_CHIIHOU_AS_DEALER = "chiihou_as_dealer_not_allowed";
        public static string ERR_CHIIHOU_WITHOUT_TSUMO = "chiihou_without_tsumo_not_allowed";
        public static string ERR_CHIIHOU_WITH_MELD = "chiihou_with_meld_not_allowed";
        public static string ERR_RENHOU_AS_DEALER = "renhou_as_dealer_not_allowed";
        public static string ERR_RENHOU_WITH_TSUMO = "renhou_with_tsumo_not_allowed";
        public static string ERR_RENHOU_WITH_MELD = "renhou_with_meld_not_allowed";

        public HandCalculator()
        {
            this.divider = new HandDivider();
        }

        // 
        //         :param tiles: array with 14 tiles in 136-tile format
        //         :param win_tile: 136 format tile that caused win (ron or tsumo)
        //         :param melds: array with Meld objects
        //         :param dora_indicators: array of tiles in 136-tile format
        //         :param config: HandConfig object
        //         :param use_hand_divider_cache: could be useful if you are calculating a lot of menchin hands
        //         :return: HandResponse object
        //         
        public virtual HandResponse estimate_hand_value(
            List<int> tiles,
            int win_tile,
            List<Meld> melds = null,
            List<int> dora_indicators = null,
            HandConfig config = null,
            ScoresCalculator scores_calculator = null,
            bool use_hand_divider_cache = false)
        {
            Dictionary<string, object> calculated_hand;
            int count_of_aka_dora;
            int count_of_dora;
            List<int> tiles_for_dora;
            List<(int, string)> fu_details;
            string error = null;
            Dictionary<string, int> cost;
            int han;
            int fu;
            if ( scores_calculator == null)
            {
                scores_calculator = new ScoresCalculator();
            }
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            if (dora_indicators == null)
            {
                dora_indicators = new List<int>();
            }
            this.config = config == null ? new HandConfig() : config;
            var hand_yaku = new List<Yaku>();
            var hand_shape = new List<List<int>>();
            var tiles_34 = TilesConverter.to_34_array(tiles.ToArray());
            var is_aotenjou = scores_calculator is Aotenjou;
            var opened_melds = (from x in melds
                                where x.opened
                                select x.tiles_34).ToList();
            var all_melds = (from x in melds
                                select x.tiles_34).ToList();
            var is_open_hand = opened_melds.Count > 0;
            // special situation
            if (this.config.is_nagashi_mangan)
            {
                hand_yaku.Add(this.config.yaku.nagashi_mangan);
                fu = 30;
                han = this.config.yaku.nagashi_mangan.han_closed;
                cost = scores_calculator.calculate_scores(han, fu, this.config, false);
                return new HandResponse(cost, han, fu, hand_yaku);
            }
            if (!tiles.Contains(win_tile))
            {
                return new HandResponse(error: ERR_NO_WINNING_TILE);
            }
            if (this.config.is_riichi && !this.config.is_daburu_riichi && is_open_hand)
            {
                return new HandResponse(error: ERR_OPEN_HAND_RIICHI);
            }
            if (this.config.is_daburu_riichi && is_open_hand)
            {
                return new HandResponse(error: ERR_OPEN_HAND_DABURI);
            }
            if (this.config.is_ippatsu && !this.config.is_riichi && !this.config.is_daburu_riichi)
            {
                return new HandResponse(error: ERR_IPPATSU_WITHOUT_RIICHI);
            }
            if (this.config.is_chankan && this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_CHANKAN_WITH_TSUMO);
            }
            if (this.config.is_rinshan && !this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_RINSHAN_WITHOUT_TSUMO);
            }
            if (this.config.is_haitei && !this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_HAITEI_WITHOUT_TSUMO);
            }
            if (this.config.is_houtei && this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_HOUTEI_WITH_TSUMO);
            }
            if (this.config.is_haitei && this.config.is_rinshan)
            {
                return new HandResponse(error: ERR_HAITEI_WITH_RINSHAN);
            }
            if (this.config.is_houtei && this.config.is_chankan)
            {
                return new HandResponse(error: ERR_HOUTEI_WITH_CHANKAN);
            }
            // raise error only when player wind is defined (and is *not* EAST)
            if (this.config.is_tenhou && this.config.player_wind != 0 && !this.config.is_dealer)
            {
                return new HandResponse(error: ERR_TENHOU_NOT_AS_DEALER);
            }
            if (this.config.is_tenhou && !this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_TENHOU_WITHOUT_TSUMO);
            }
            if (this.config.is_tenhou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_TENHOU_WITH_MELD);
            }
            // raise error only when player wind is defined (and is EAST)
            if (this.config.is_chiihou && this.config.player_wind != 0 && this.config.is_dealer)
            {
                return new HandResponse(error: ERR_CHIIHOU_AS_DEALER);
            }
            if (this.config.is_chiihou && !this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_CHIIHOU_WITHOUT_TSUMO);
            }
            if (this.config.is_chiihou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_CHIIHOU_WITH_MELD);
            }
            // raise error only when player wind is defined (and is EAST)
            if (this.config.is_renhou && this.config.player_wind != 0 && this.config.is_dealer)
            {
                return new HandResponse(error: ERR_RENHOU_AS_DEALER);
            }
            if (this.config.is_renhou && this.config.is_tsumo)
            {
                return new HandResponse(error: ERR_RENHOU_WITH_TSUMO);
            }
            if (this.config.is_renhou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_RENHOU_WITH_MELD);
            }
            if (!Agari.IsAgari(tiles_34, all_melds))
            {
                return new HandResponse(error: ERR_HAND_NOT_WINNING);
            }
            if (!this.config.options.has_double_yakuman)
            {
                this.config.yaku.daburu_kokushi.han_closed = 13;
                this.config.yaku.suuankou_tanki.han_closed = 13;
                this.config.yaku.daburu_chuuren_poutou.han_closed = 13;
                this.config.yaku.daisuushi.han_closed = 13;
                this.config.yaku.daisuushi.han_open = 13;
            }
            var hand_options = this.divider.divide_hand(tiles_34, melds, use_cache: use_hand_divider_cache);
            var calculated_hands = new List<Dictionary<string, object>>();
            foreach (var hand in hand_options)
            {
                var is_chiitoitsu = this.config.yaku.chiitoitsu.is_condition_met(hand);
                var valued_tiles = new List<int> {
                    C.HAKU,
                    C.HATSU,
                    C.CHUN,
                    this.config.player_wind,
                    this.config.round_wind
                };
                var win_groups = this._find_win_groups(win_tile, hand, opened_melds);
                foreach (var win_group in win_groups)
                {
                    cost = null;
                    error = null;
                    hand_yaku = new List<Yaku>();
                    han = 0;
                    var _tup_1 = FuCalculator.calculate_fu(hand, win_tile, win_group, this.config, valued_tiles, melds);
                    fu_details = _tup_1.Item1;
                    fu = _tup_1.Item2;
                    var is_pinfu = fu_details.Count == 1 && !is_chiitoitsu && !is_open_hand;
                    var pon_sets = (from x in hand
                                    where U.is_pon(x)
                                    select x).ToList();
                    var kan_sets = (from x in hand
                                    where U.is_kan(x)
                                    select x).ToList();
                    var chi_sets = (from x in hand
                                    where U.is_chi(x)
                                    select x).ToList();
                    if (this.config.is_tsumo)
                    {
                        if (!is_open_hand)
                        {
                            hand_yaku.Add(this.config.yaku.tsumo);
                        }
                    }
                    if (is_pinfu)
                    {
                        hand_yaku.Add(this.config.yaku.pinfu);
                    }
                    // let's skip hand that looks like chitoitsu, but it contains open sets
                    if (is_chiitoitsu && is_open_hand)
                    {
                        continue;
                    }
                    if (is_chiitoitsu)
                    {
                        hand_yaku.Add(this.config.yaku.chiitoitsu);
                    }
                    var is_daisharin = this.config.yaku.daisharin.is_condition_met(hand, this.config.options.has_daisharin_other_suits);
                    if (this.config.options.has_daisharin && is_daisharin)
                    {
                        this.config.yaku.daisharin.rename(hand);
                        hand_yaku.Add(this.config.yaku.daisharin);
                    }
                    if (this.config.options.has_daichisei && this.config.yaku.daichisei.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.daichisei);
                    }
                    var is_tanyao = this.config.yaku.tanyao.is_condition_met(hand);
                    if (is_open_hand && !this.config.options.has_open_tanyao)
                    {
                        is_tanyao = false;
                    }
                    if (is_tanyao)
                    {
                        hand_yaku.Add(this.config.yaku.tanyao);
                    }
                    if (this.config.is_riichi && !this.config.is_daburu_riichi)
                    {
                        if (this.config.is_open_riichi)
                        {
                            hand_yaku.Add(this.config.yaku.open_riichi);
                        }
                        else
                        {
                            hand_yaku.Add(this.config.yaku.riichi);
                        }
                    }
                    if (this.config.is_daburu_riichi)
                    {
                        if (this.config.is_open_riichi)
                        {
                            hand_yaku.Add(this.config.yaku.daburu_open_riichi);
                        }
                        else
                        {
                            hand_yaku.Add(this.config.yaku.daburu_riichi);
                        }
                    }
                    if (!this.config.is_tsumo && this.config.options.has_sashikomi_yakuman && (hand_yaku.Contains(this.config.yaku.daburu_open_riichi) || hand_yaku.Contains(this.config.yaku.open_riichi)))
                    {
                        hand_yaku.Add(this.config.yaku.sashikomi);
                    }
                    if (this.config.is_ippatsu)
                    {
                        hand_yaku.Add(this.config.yaku.ippatsu);
                    }
                    if (this.config.is_rinshan)
                    {
                        hand_yaku.Add(this.config.yaku.rinshan);
                    }
                    if (this.config.is_chankan)
                    {
                        hand_yaku.Add(this.config.yaku.chankan);
                    }
                    if (this.config.is_haitei)
                    {
                        hand_yaku.Add(this.config.yaku.haitei);
                    }
                    if (this.config.is_houtei)
                    {
                        hand_yaku.Add(this.config.yaku.houtei);
                    }
                    if (this.config.is_renhou)
                    {
                        if (this.config.options.renhou_as_yakuman)
                        {
                            hand_yaku.Add(this.config.yaku.renhou_yakuman);
                        }
                        else
                        {
                            hand_yaku.Add(this.config.yaku.renhou);
                        }
                    }
                    if (this.config.is_tenhou)
                    {
                        hand_yaku.Add(this.config.yaku.tenhou);
                    }
                    if (this.config.is_chiihou)
                    {
                        hand_yaku.Add(this.config.yaku.chiihou);
                    }
                    if (this.config.yaku.honitsu.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.honitsu);
                    }
                    if (this.config.yaku.chinitsu.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.chinitsu);
                    }
                    if (this.config.yaku.tsuisou.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.tsuisou);
                    }
                    if (this.config.yaku.honroto.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.honroto);
                    }
                    if (this.config.yaku.chinroto.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.chinroto);
                    }
                    if (this.config.yaku.ryuisou.is_condition_met(hand))
                    {
                        hand_yaku.Add(this.config.yaku.ryuisou);
                    }
                    if (this.config.paarenchan > 0 && !this.config.options.paarenchan_needs_yaku)
                    {
                        // if no yaku is even needed to win on paarenchan and it is paarenchan condition, just add paarenchan
                        this.config.yaku.paarenchan.set_paarenchan_count(this.config.paarenchan);
                        hand_yaku.Add(this.config.yaku.paarenchan);
                    }
                    // small optimization, try to detect yaku with chi required sets only if we have chi sets in hand
                    if (chi_sets.Count()>0)
                    {
                        if (this.config.yaku.chantai.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.chantai);
                        }
                        if (this.config.yaku.junchan.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.junchan);
                        }
                        if (this.config.yaku.ittsu.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.ittsu);
                        }
                        if (!is_open_hand)
                        {
                            if (this.config.yaku.ryanpeiko.is_condition_met(hand))
                            {
                                hand_yaku.Add(this.config.yaku.ryanpeiko);
                            }
                            else if (this.config.yaku.iipeiko.is_condition_met(hand))
                            {
                                hand_yaku.Add(this.config.yaku.iipeiko);
                            }
                        }
                        if (this.config.yaku.sanshoku.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.sanshoku);
                        }
                    }
                    // small optimization, try to detect yaku with pon required sets only if we have pon sets in hand
                    if (pon_sets.Count() > 0 || kan_sets.Count() > 0)
                    {
                        if (this.config.yaku.toitoi.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.toitoi);
                        }
                        if (this.config.yaku.sanankou.is_condition_met(hand, win_tile, melds, this.config.is_tsumo))
                        {
                            hand_yaku.Add(this.config.yaku.sanankou);
                        }
                        if (this.config.yaku.sanshoku_douko.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.sanshoku_douko);
                        }
                        if (this.config.yaku.shosangen.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.shosangen);
                        }
                        if (this.config.yaku.haku.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.haku);
                        }
                        if (this.config.yaku.hatsu.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.hatsu);
                        }
                        if (this.config.yaku.chun.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.chun);
                        }
                        if (this.config.yaku.east.is_condition_met(hand, this.config.player_wind, this.config.round_wind))
                        {
                            if (this.config.player_wind == C.EAST)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_place);
                            }
                            if (this.config.round_wind == C.EAST)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_round);
                            }
                        }
                        if (this.config.yaku.south.is_condition_met(hand, this.config.player_wind, this.config.round_wind))
                        {
                            if (this.config.player_wind == C.SOUTH)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_place);
                            }
                            if (this.config.round_wind == C.SOUTH)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_round);
                            }
                        }
                        if (this.config.yaku.west.is_condition_met(hand, this.config.player_wind, this.config.round_wind))
                        {
                            if (this.config.player_wind == C.WEST)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_place);
                            }
                            if (this.config.round_wind == C.WEST)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_round);
                            }
                        }
                        if (this.config.yaku.north.is_condition_met(hand, this.config.player_wind, this.config.round_wind))
                        {
                            if (this.config.player_wind == C.NORTH)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_place);
                            }
                            if (this.config.round_wind == C.NORTH)
                            {
                                hand_yaku.Add(this.config.yaku.yakuhai_round);
                            }
                        }
                        if (this.config.yaku.daisangen.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.daisangen);
                        }
                        if (this.config.yaku.shosuushi.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.shosuushi);
                        }
                        if (this.config.yaku.daisuushi.is_condition_met(hand))
                        {
                            hand_yaku.Add(this.config.yaku.daisuushi);
                        }
                        // closed kan can't be used in chuuren_poutou
                        if (melds.Count == 0 && this.config.yaku.chuuren_poutou.is_condition_met(hand))
                        {
                            if (tiles_34[win_tile / 4] == 2 || tiles_34[win_tile / 4] == 4)
                            {
                                hand_yaku.Add(this.config.yaku.daburu_chuuren_poutou);
                            }
                            else
                            {
                                hand_yaku.Add(this.config.yaku.chuuren_poutou);
                            }
                        }
                        if (!is_open_hand && this.config.yaku.suuankou.is_condition_met(hand, win_tile, this.config.is_tsumo))
                        {
                            if (tiles_34[win_tile / 4] == 2)
                            {
                                hand_yaku.Add(this.config.yaku.suuankou_tanki);
                            }
                            else
                            {
                                hand_yaku.Add(this.config.yaku.suuankou);
                            }
                        }
                        if (this.config.yaku.sankantsu.is_condition_met(hand, melds))
                        {
                            hand_yaku.Add(this.config.yaku.sankantsu);
                        }
                        if (this.config.yaku.suukantsu.is_condition_met(hand, melds))
                        {
                            hand_yaku.Add(this.config.yaku.suukantsu);
                        }
                    }
                    if (this.config.paarenchan > 0 && this.config.options.paarenchan_needs_yaku && hand_yaku.Count > 0)
                    {
                        // we waited until here to add paarenchan yakuman only if there is any other yaku
                        this.config.yaku.paarenchan.set_paarenchan_count(this.config.paarenchan);
                        hand_yaku.Add(this.config.yaku.paarenchan);
                    }
                    // yakuman is not connected with other yaku
                    var yakuman_list = (from x in hand_yaku
                                        where x.is_yakuman
                                        select x).ToList();
                    if (yakuman_list.Count() > 0)
                    {
                        if (!is_aotenjou)
                        {
                            hand_yaku = yakuman_list;
                        }
                        else
                        {
                            ScoresCalculator.aotenjou_filter_yaku(hand_yaku, this.config);
                            yakuman_list = new List<Yaku>();
                        }
                    }
                    // calculate han
                    foreach (var item in hand_yaku)
                    {
                        if (is_open_hand && item.han_open > 0)
                        {
                            han += item.han_open;
                        }
                        else
                        {
                            han += item.han_closed;
                        }
                    }
                    if (han == 0)
                    {
                        error = HandCalculator.ERR_NO_YAKU;
                        cost = null;
                    }
                    // we don't need to add dora to yakuman
                    if (!(yakuman_list.Count()>0))
                    {
                        tiles_for_dora = tiles.ToArray().ToList();
                        count_of_dora = 0;
                        count_of_aka_dora = 0;
                        foreach (var tile in tiles_for_dora)
                        {
                            count_of_dora += U.plus_dora(tile, dora_indicators);
                        }
                        foreach (var tile in tiles_for_dora)
                        {
                            if (U.is_aka_dora(tile, this.config.options.has_aka_dora))
                            {
                                count_of_aka_dora += 1;
                            }
                        }
                        if (count_of_dora>0)
                        {
                            this.config.yaku.dora.han_open = count_of_dora;
                            this.config.yaku.dora.han_closed = count_of_dora;
                            hand_yaku.Add(this.config.yaku.dora);
                            han += count_of_dora;
                        }
                        if (count_of_aka_dora>0)
                        {
                            this.config.yaku.aka_dora.han_open = count_of_aka_dora;
                            this.config.yaku.aka_dora.han_closed = count_of_aka_dora;
                            hand_yaku.Add(this.config.yaku.aka_dora);
                            han += count_of_aka_dora;
                        }
                    }
                    if (!is_aotenjou && (this.config.options.limit_to_sextuple_yakuman && han > 78))
                    {
                        han = 78;
                    }
                    if (fu == 0 && is_aotenjou)
                    {
                        fu = 40;
                    }
                    if (error != null)
                    {
                        cost = scores_calculator.calculate_scores(han, fu, this.config, yakuman_list.Count > 0);
                    }
                    calculated_hand = new Dictionary<string, object> {
                        {
                            "cost",
                            cost},
                        {
                            "error",
                            error},
                        {
                            "hand_yaku",
                            hand_yaku},
                        {
                            "hand_shape",
                            hand},
                        {
                            "han",
                            han},
                        {
                            "fu",
                            fu},
                        {
                            "fu_details",
                            fu_details}};
                    calculated_hands.Add(calculated_hand);
                }
            }
            // exception hand
            if (!is_open_hand && this.config.yaku.kokushi.is_condition_met(null, tiles_34))
            {
                if (tiles_34[win_tile / 4] == 2)
                {
                    hand_yaku.Add(this.config.yaku.daburu_kokushi);
                }
                else
                {
                    hand_yaku.Add(this.config.yaku.kokushi);
                }
                if (!this.config.is_tsumo && this.config.options.has_sashikomi_yakuman)
                {
                    if (this.config.is_riichi && !this.config.is_daburu_riichi)
                    {
                        if (this.config.is_open_riichi)
                        {
                            hand_yaku.Add(this.config.yaku.sashikomi);
                        }
                    }
                    if (this.config.is_daburu_riichi)
                    {
                        if (this.config.is_open_riichi)
                        {
                            hand_yaku.Add(this.config.yaku.sashikomi);
                        }
                    }
                }
                if (this.config.is_renhou && this.config.options.renhou_as_yakuman)
                {
                    hand_yaku.Add(this.config.yaku.renhou_yakuman);
                }
                if (this.config.is_tenhou)
                {
                    hand_yaku.Add(this.config.yaku.tenhou);
                }
                if (this.config.is_chiihou)
                {
                    hand_yaku.Add(this.config.yaku.chiihou);
                }
                if (this.config.paarenchan > 0)
                {
                    this.config.yaku.paarenchan.set_paarenchan_count(this.config.paarenchan);
                    hand_yaku.Add(this.config.yaku.paarenchan);
                }
                // calculate han
                han = 0;
                foreach (var item in hand_yaku)
                {
                    if (is_open_hand && item.han_open>0)
                    {
                        han += item.han_open;
                    }
                    else
                    {
                        han += item.han_closed;
                    }
                }
                fu = 0;
                if (is_aotenjou)
                {
                    if (this.config.is_tsumo)
                    {
                        fu = 30;
                    }
                    else
                    {
                        fu = 40;
                    }
                    tiles_for_dora = tiles.ToArray().ToList();
                    count_of_dora = 0;
                    count_of_aka_dora = 0;
                    foreach (var tile in tiles_for_dora)
                    {
                        count_of_dora += U.plus_dora(tile, dora_indicators);
                    }
                    foreach (var tile in tiles_for_dora)
                    {
                        if (U.is_aka_dora(tile, this.config.options.has_aka_dora))
                        {
                            count_of_aka_dora += 1;
                        }
                    }
                    if (count_of_dora>0)
                    {
                        this.config.yaku.dora.han_open = count_of_dora;
                        this.config.yaku.dora.han_closed = count_of_dora;
                        hand_yaku.Add(this.config.yaku.dora);
                        han += count_of_dora;
                    }
                    if (count_of_aka_dora>0)
                    {
                        this.config.yaku.aka_dora.han_open = count_of_aka_dora;
                        this.config.yaku.aka_dora.han_closed = count_of_aka_dora;
                        hand_yaku.Add(this.config.yaku.aka_dora);
                        han += count_of_aka_dora;
                    }
                }
                cost = scores_calculator.calculate_scores(han, fu, this.config, hand_yaku.Count > 0);
                calculated_hands.Add(new Dictionary<string, object> {
                    {
                        "cost",
                        cost},
                    {
                        "error",
                        null},
                    {
                        "hand_yaku",
                        hand_yaku},
                    {
                        "han",
                        han},
                    {
                        "fu",
                        fu},
                    {
                        "fu_details",
                        new List<(int, string)>()}});
            }
            if (!(calculated_hands.Count >0))
            {
                return new HandResponse(error: HandCalculator.ERR_HAND_NOT_CORRECT);
            }
            // let's use cost for most expensive hand
            calculated_hands = calculated_hands.OrderByDescending(x => (x["han"], x["fu"])).ToList();
            calculated_hand = calculated_hands[0];
            cost = (Dictionary<string,int>)calculated_hand["cost"];
            error = (string)calculated_hand["error"];
            hand_yaku = (List<Yaku>)calculated_hand["hand_yaku"];
            han = (int)calculated_hand["han"];
            fu = (int)calculated_hand["fu"];
            hand_shape = (List<List<int>>)calculated_hand["hand_shape"];
            fu_details = (List<(int, string)>)calculated_hand["fu_details"];
            return new HandResponse(cost, han, fu, hand_yaku, hand_shape, error, fu_details, is_open_hand);
        }

        List<List<int>> _find_win_groups(int win_tile, List<List<int>> hand, List<List<int>> opened_melds)
        {
            var win_tile_34 = win_tile / 4;
            // to detect win groups
            // we had to use only closed sets
            var closed_set_items = new List<List<int>>();
            foreach (var x in hand)
            {
                if (!opened_melds.Contains(x))
                {
                    closed_set_items.Add(x);
                }
                else
                {
                    opened_melds.Remove(x);
                }
            }
            // for forms like 45666 and ron on 6
            // we can assume that ron was on 456 form and on 66 form
            // and depends on form we will have different hand cost
            // so, we had to check all possible win groups
            var win_groups = (from x in closed_set_items
                                where x.Contains(win_tile_34)
                                select x).ToList();
            win_groups.Distinct(new GroupComparer<int>());
            return win_groups;
        }
    }
    
}
