using kandora.bot.utils;
using System.Collections.Generic;
using System.Linq;
using C = kandora.bot.mahjong.Constants;
using U = kandora.bot.mahjong.Utils;

namespace kandora.bot.mahjong.handcalc
{

    public class HandCalculator
    {
        public HandConfig config = null;
        HandDivider handDivider = null;
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
            this.handDivider = new HandDivider();
        } 

        /// <summary>
        /// Compute the value in han and fu of a hand by taking the best of all possible ways of looking at it
        /// </summary>
        /// <param name="tiles">14 tiles list of 136 format</param>
        /// <param name="winTile">Winning tile, 136 format</param>
        /// <param name="melds">The list of the melds</param>
        /// <param name="doraIndicators">List of 136 format tiles</param>
        /// <param name="config">Current hand config</param>
        /// <param name="scoresCalculator">Score calculator object</param>
        /// <param name="useCache">True: Do not compute the same hand twice</param>
        /// <returns>A HandResponse object</returns>
        public virtual HandResponse EstimateHandValue(
            List<int> tiles,
            int winTile,
            List<Meld> melds = null,
            List<int> doraIndicators = null,
            HandConfig config = null,
            ScoresCalculator scoresCalculator = null,
            bool useCache = false)
        {
            Dictionary<string, object> calculatedHand;
            int akaDoraCount;
            int doraCount;
            List<int> tilesForDora;
            List<(int, string)> fuDetails;
            string error = null;
            Dictionary<string, int> cost;
            int han;
            int fu;
            if ( scoresCalculator == null)
            {
                scoresCalculator = new ScoresCalculator();
            }
            if (melds == null)
            {
                melds = new List<Meld>();
            }
            if (doraIndicators == null)
            {
                doraIndicators = new List<int>();
            }
            this.config = config == null ? new HandConfig() : config;
            var yakusInHand = new List<Yaku>();
            var handShape = new List<List<int>>();
            var tiles34 = TilesConverter.From136to34count(tiles.ToArray());
            var isAotenjou = scoresCalculator is Aotenjou;
            var openedMelds34 = (from x in melds
                                where x.opened
                                select x.Tiles34).ToList();
            var allMelds = (from x in melds
                                select x.Tiles34).ToList();
            var isOpenHand = openedMelds34.Count > 0;
            // special situation
            if (this.config.isNagashiMangan)
            {
                yakusInHand.Add(this.config.yaku.nagashiMangan);
                fu = 30;
                han = this.config.yaku.nagashiMangan.nbHanClosed;
                cost = scoresCalculator.CalculateScores(han, fu, this.config, false);
                return new HandResponse(cost, han, fu, yakusInHand);
            }
            if (!tiles.Contains(winTile))
            {
                return new HandResponse(error: ERR_NO_WINNING_TILE);
            }
            if (this.config.isRiichi && !this.config.isDaburuRiichi && isOpenHand)
            {
                return new HandResponse(error: ERR_OPEN_HAND_RIICHI);
            }
            if (this.config.isDaburuRiichi && isOpenHand)
            {
                return new HandResponse(error: ERR_OPEN_HAND_DABURI);
            }
            if (this.config.isIppatsu && !this.config.isRiichi && !this.config.isDaburuRiichi)
            {
                return new HandResponse(error: ERR_IPPATSU_WITHOUT_RIICHI);
            }
            if (this.config.isChankan && this.config.isTsumo)
            {
                return new HandResponse(error: ERR_CHANKAN_WITH_TSUMO);
            }
            if (this.config.isRinshan && !this.config.isTsumo)
            {
                return new HandResponse(error: ERR_RINSHAN_WITHOUT_TSUMO);
            }
            if (this.config.isHaitei && !this.config.isTsumo)
            {
                return new HandResponse(error: ERR_HAITEI_WITHOUT_TSUMO);
            }
            if (this.config.isHoutei && this.config.isTsumo)
            {
                return new HandResponse(error: ERR_HOUTEI_WITH_TSUMO);
            }
            if (this.config.isHaitei && this.config.isRinshan)
            {
                return new HandResponse(error: ERR_HAITEI_WITH_RINSHAN);
            }
            if (this.config.isHoutei && this.config.isChankan)
            {
                return new HandResponse(error: ERR_HOUTEI_WITH_CHANKAN);
            }
            // raise error only when player wind is defined (and is *not* EAST)
            if (this.config.isTenhou && this.config.playerWind != 0 && !this.config.isDealer)
            {
                return new HandResponse(error: ERR_TENHOU_NOT_AS_DEALER);
            }
            if (this.config.isTenhou && !this.config.isTsumo)
            {
                return new HandResponse(error: ERR_TENHOU_WITHOUT_TSUMO);
            }
            if (this.config.isTenhou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_TENHOU_WITH_MELD);
            }
            // raise error only when player wind is defined (and is EAST)
            if (this.config.isChiihou && this.config.playerWind != 0 && this.config.isDealer)
            {
                return new HandResponse(error: ERR_CHIIHOU_AS_DEALER);
            }
            if (this.config.isChiihou && !this.config.isTsumo)
            {
                return new HandResponse(error: ERR_CHIIHOU_WITHOUT_TSUMO);
            }
            if (this.config.isChiihou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_CHIIHOU_WITH_MELD);
            }
            // raise error only when player wind is defined (and is EAST)
            if (this.config.isRenhou && this.config.playerWind != 0 && this.config.isDealer)
            {
                return new HandResponse(error: ERR_RENHOU_AS_DEALER);
            }
            if (this.config.isRenhou && this.config.isTsumo)
            {
                return new HandResponse(error: ERR_RENHOU_WITH_TSUMO);
            }
            if (this.config.isRenhou && (melds != null || melds.Count > 0))
            {
                return new HandResponse(error: ERR_RENHOU_WITH_MELD);
            }
            if (!Agari.IsAgari(tiles34, allMelds))
            {
                return new HandResponse(error: ERR_HAND_NOT_WINNING);
            }
            if (!this.config.options.hasDoubleYakuman)
            {
                this.config.yaku.daburuKokushi.nbHanClosed = 13;
                this.config.yaku.suuankouTanki.nbHanClosed = 13;
                this.config.yaku.daburuChuurenPoutou.nbHanClosed = 13;
                this.config.yaku.daisuushi.nbHanClosed = 13;
                this.config.yaku.daisuushi.nbHanOpen = 13;
            }
            var handOptions = handDivider.DivideHand(tiles34, melds, useCache: useCache);
            var calculatedHands = new List<Dictionary<string, object>>();
            foreach (var hand in handOptions)
            {
                var isChiitoitsu = this.config.yaku.chiitoitsu.isConditionMet(hand);
                var valued_tiles = new List<int> {
                    C.HAKU,
                    C.HATSU,
                    C.CHUN,
                    this.config.playerWind,
                    this.config.roundWind
                };
                var winGroups = FindWinGroups(winTile, hand, openedMelds34);
                foreach (var winGroup in winGroups)
                {
                    cost = null;
                    error = null;
                    yakusInHand = new List<Yaku>();
                    han = 0;
                    var calculatedFu = FuCalculator.CalculateFu(hand, winTile, winGroup, this.config, valued_tiles, melds);
                    fuDetails = calculatedFu.Item1;
                    fu = calculatedFu.Item2;
                    var isPinfu = fuDetails.Count == 1 && !isChiitoitsu && !isOpenHand;
                    var koutsuList = (from x in hand
                                    where U.IsKoutsu(x)
                                    select x).ToList();
                    var kantsuList = (from x in hand
                                    where U.IsKantsu(x)
                                    select x).ToList();
                    var shoutsuList = (from x in hand
                                    where U.IsShuntsu(x)
                                    select x).ToList();
                    if (this.config.isTsumo)
                    {
                        if (!isOpenHand)
                        {
                            yakusInHand.Add(this.config.yaku.tsumo);
                        }
                    }
                    if (isPinfu)
                    {
                        yakusInHand.Add(this.config.yaku.pinfu);
                    }
                    // let's skip hand that looks like chitoitsu, but it contains open sets
                    if (isChiitoitsu && isOpenHand)
                    {
                        continue;
                    }
                    if (isChiitoitsu)
                    {
                        yakusInHand.Add(this.config.yaku.chiitoitsu);
                    }
                    var isDaisharin = this.config.yaku.daisharin.isConditionMet(hand, this.config.options.hasDaisharinOtherSuits);
                    if (this.config.options.hasDaisharin && isDaisharin)
                    {
                        this.config.yaku.daisharin.rename(hand);
                        yakusInHand.Add(this.config.yaku.daisharin);
                    }
                    if (this.config.options.hasDaichisei && this.config.yaku.daichisei.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.daichisei);
                    }
                    var isTanyao = this.config.yaku.tanyao.isConditionMet(hand);
                    if (isOpenHand && !this.config.options.hasOpenTanyao)
                    {
                        isTanyao = false;
                    }
                    if (isTanyao)
                    {
                        yakusInHand.Add(this.config.yaku.tanyao);
                    }
                    if (this.config.isRiichi && !this.config.isDaburuRiichi)
                    {
                        if (this.config.isOpenRiichi)
                        {
                            yakusInHand.Add(this.config.yaku.openRiichi);
                        }
                        else
                        {
                            yakusInHand.Add(this.config.yaku.riichi);
                        }
                    }
                    if (this.config.isDaburuRiichi)
                    {
                        if (this.config.isOpenRiichi)
                        {
                            yakusInHand.Add(this.config.yaku.daburuOpenRiichi);
                        }
                        else
                        {
                            yakusInHand.Add(this.config.yaku.daburuRiichi);
                        }
                    }
                    if (!this.config.isTsumo && this.config.options.hasSashikomiYakuman && (yakusInHand.Contains(this.config.yaku.daburuOpenRiichi) || yakusInHand.Contains(this.config.yaku.openRiichi)))
                    {
                        yakusInHand.Add(this.config.yaku.sashikomi);
                    }
                    if (this.config.isIppatsu)
                    {
                        yakusInHand.Add(this.config.yaku.ippatsu);
                    }
                    if (this.config.isRinshan)
                    {
                        yakusInHand.Add(this.config.yaku.rinshan);
                    }
                    if (this.config.isChankan)
                    {
                        yakusInHand.Add(this.config.yaku.chankan);
                    }
                    if (this.config.isHaitei)
                    {
                        yakusInHand.Add(this.config.yaku.haitei);
                    }
                    if (this.config.isHoutei)
                    {
                        yakusInHand.Add(this.config.yaku.houtei);
                    }
                    if (this.config.isRenhou)
                    {
                        if (this.config.options.renhouAsYakuman)
                        {
                            yakusInHand.Add(this.config.yaku.renhouYakuman);
                        }
                        else
                        {
                            yakusInHand.Add(this.config.yaku.renhou);
                        }
                    }
                    if (this.config.isTenhou)
                    {
                        yakusInHand.Add(this.config.yaku.tenhou);
                    }
                    if (this.config.isChiihou)
                    {
                        yakusInHand.Add(this.config.yaku.chiihou);
                    }
                    if (this.config.yaku.honitsu.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.honitsu);
                    }
                    if (this.config.yaku.chinitsu.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.chinitsu);
                    }
                    if (this.config.yaku.tsuisou.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.tsuisou);
                    }
                    if (this.config.yaku.honroto.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.honroto);
                    }
                    if (this.config.yaku.chinroto.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.chinroto);
                    }
                    if (this.config.yaku.ryuisou.isConditionMet(hand))
                    {
                        yakusInHand.Add(this.config.yaku.ryuisou);
                    }
                    if (this.config.paarenchan > 0 && !this.config.options.paarenchanNeedsYaku)
                    {
                        // if no yaku is even needed to win on paarenchan and it is paarenchan condition, just add paarenchan
                        this.config.yaku.paarenchan.setPaarenchanCount(this.config.paarenchan);
                        yakusInHand.Add(this.config.yaku.paarenchan);
                    }
                    // small optimization, try to detect yaku with chi required sets only if we have chi sets in hand
                    if (shoutsuList.Count()>0)
                    {
                        if (this.config.yaku.chantai.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.chantai);
                        }
                        if (this.config.yaku.junchan.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.junchan);
                        }
                        if (this.config.yaku.ittsu.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.ittsu);
                        }
                        if (!isOpenHand)
                        {
                            if (this.config.yaku.ryanpeiko.isConditionMet(hand))
                            {
                                yakusInHand.Add(this.config.yaku.ryanpeiko);
                            }
                            else if (this.config.yaku.iipeiko.isConditionMet(hand))
                            {
                                yakusInHand.Add(this.config.yaku.iipeiko);
                            }
                        }
                        if (this.config.yaku.sanshoku.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.sanshoku);
                        }
                    }
                    // small optimization, try to detect yaku with pon required sets only if we have pon sets in hand
                    if (koutsuList.Count() > 0 || kantsuList.Count() > 0)
                    {
                        if (this.config.yaku.toitoi.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.toitoi);
                        }
                        if (this.config.yaku.sanankou.isConditionMet(hand, winTile, melds, this.config.isTsumo))
                        {
                            yakusInHand.Add(this.config.yaku.sanankou);
                        }
                        if (this.config.yaku.sanshokuDoukou.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.sanshokuDoukou);
                        }
                        if (this.config.yaku.shosangen.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.shosangen);
                        }
                        if (this.config.yaku.haku.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.haku);
                        }
                        if (this.config.yaku.hatsu.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.hatsu);
                        }
                        if (this.config.yaku.chun.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.chun);
                        }
                        if (this.config.yaku.east.isConditionMet(hand, this.config.playerWind, this.config.roundWind))
                        {
                            if (this.config.playerWind == C.EAST)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiPlace);
                            }
                            if (this.config.roundWind == C.EAST)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiRound);
                            }
                        }
                        if (this.config.yaku.south.isConditionMet(hand, this.config.playerWind, this.config.roundWind))
                        {
                            if (this.config.playerWind == C.SOUTH)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiPlace);
                            }
                            if (this.config.roundWind == C.SOUTH)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiRound);
                            }
                        }
                        if (this.config.yaku.west.isConditionMet(hand, this.config.playerWind, this.config.roundWind))
                        {
                            if (this.config.playerWind == C.WEST)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiPlace);
                            }
                            if (this.config.roundWind == C.WEST)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiRound);
                            }
                        }
                        if (this.config.yaku.north.isConditionMet(hand, this.config.playerWind, this.config.roundWind))
                        {
                            if (this.config.playerWind == C.NORTH)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiPlace);
                            }
                            if (this.config.roundWind == C.NORTH)
                            {
                                yakusInHand.Add(this.config.yaku.yakuhaiRound);
                            }
                        }
                        if (this.config.yaku.daisangen.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.daisangen);
                        }
                        if (this.config.yaku.shosuushi.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.shosuushi);
                        }
                        if (this.config.yaku.daisuushi.isConditionMet(hand))
                        {
                            yakusInHand.Add(this.config.yaku.daisuushi);
                        }
                        // closed kan can't be used in chuurenPoutou
                        if (melds.Count == 0 && this.config.yaku.chuurenPoutou.isConditionMet(hand))
                        {
                            if (tiles34[winTile / 4] == 2 || tiles34[winTile / 4] == 4)
                            {
                                yakusInHand.Add(this.config.yaku.daburuChuurenPoutou);
                            }
                            else
                            {
                                yakusInHand.Add(this.config.yaku.chuurenPoutou);
                            }
                        }
                        if (!isOpenHand && this.config.yaku.suuankou.isConditionMet(hand, winTile, this.config.isTsumo))
                        {
                            if (tiles34[winTile / 4] == 2)
                            {
                                yakusInHand.Add(this.config.yaku.suuankouTanki);
                            }
                            else
                            {
                                yakusInHand.Add(this.config.yaku.suuankou);
                            }
                        }
                        if (this.config.yaku.sankantsu.isConditionMet(hand, melds))
                        {
                            yakusInHand.Add(this.config.yaku.sankantsu);
                        }
                        if (this.config.yaku.suukantsu.isConditionMet(hand, melds))
                        {
                            yakusInHand.Add(this.config.yaku.suukantsu);
                        }
                    }
                    if (this.config.paarenchan > 0 && this.config.options.paarenchanNeedsYaku && yakusInHand.Count > 0)
                    {
                        // we waited until here to add paarenchan yakuman only if there is any other yaku
                        this.config.yaku.paarenchan.setPaarenchanCount(this.config.paarenchan);
                        yakusInHand.Add(this.config.yaku.paarenchan);
                    }
                    // yakuman is not connected with other yaku
                    var yakumanList = (from x in yakusInHand
                                        where x.isYakuman
                                        select x).ToList();
                    if (yakumanList.Count() > 0)
                    {
                        if (!isAotenjou)
                        {
                            yakusInHand = yakumanList;
                        }
                        else
                        {
                            ScoresCalculator.aotenjouFilterYaku(yakusInHand, this.config);
                            yakumanList = new List<Yaku>();
                        }
                    }
                    // calculate han
                    foreach (var item in yakusInHand)
                    {
                        if (isOpenHand && item.nbHanOpen > 0)
                        {
                            han += item.nbHanOpen;
                        }
                        else
                        {
                            han += item.nbHanClosed;
                        }
                    }
                    if (han == 0)
                    {
                        error = HandCalculator.ERR_NO_YAKU;
                        cost = null;
                    }
                    // we don't need to add dora to yakuman
                    if (!(yakumanList.Count()>0))
                    {
                        tilesForDora = tiles.ToArray().ToList();
                        doraCount = 0;
                        akaDoraCount = 0;
                        foreach (var tile in tilesForDora)
                        {
                            doraCount += U.PlusDora(tile, doraIndicators);
                        }
                        foreach (var tile in tilesForDora)
                        {
                            if (U.IsAkaDora(tile, this.config.options.hasAkaDora))
                            {
                                akaDoraCount += 1;
                            }
                        }
                        if (doraCount>0)
                        {
                            this.config.yaku.dora.nbHanOpen = doraCount;
                            this.config.yaku.dora.nbHanClosed = doraCount;
                            yakusInHand.Add(this.config.yaku.dora);
                            han += doraCount;
                        }
                        if (akaDoraCount>0)
                        {
                            this.config.yaku.akaDora.nbHanOpen = akaDoraCount;
                            this.config.yaku.akaDora.nbHanClosed = akaDoraCount;
                            yakusInHand.Add(this.config.yaku.akaDora);
                            han += akaDoraCount;
                        }
                    }
                    if (!isAotenjou && (this.config.options.limitToSextupleYakuman && han > 78))
                    {
                        han = 78;
                    }
                    if (fu == 0 && isAotenjou)
                    {
                        fu = 40;
                    }
                    if (error != null)
                    {
                        cost = scoresCalculator.CalculateScores(han, fu, this.config, yakumanList.Count > 0);
                    }
                    calculatedHand = new Dictionary<string, object> {
                        {
                            "cost",
                            cost},
                        {
                            "error",
                            error},
                        {
                            "hand_yaku",
                            yakusInHand},
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
                            fuDetails}};
                    calculatedHands.Add(calculatedHand);
                }
            }
            // exception hand
            if (!isOpenHand && this.config.yaku.kokushi.isConditionMet(null, tiles34))
            {
                if (tiles34[winTile / 4] == 2)
                {
                    yakusInHand.Add(this.config.yaku.daburuKokushi);
                }
                else
                {
                    yakusInHand.Add(this.config.yaku.kokushi);
                }
                if (!this.config.isTsumo && this.config.options.hasSashikomiYakuman)
                {
                    if (this.config.isRiichi && !this.config.isDaburuRiichi)
                    {
                        if (this.config.isOpenRiichi)
                        {
                            yakusInHand.Add(this.config.yaku.sashikomi);
                        }
                    }
                    if (this.config.isDaburuRiichi)
                    {
                        if (this.config.isOpenRiichi)
                        {
                            yakusInHand.Add(this.config.yaku.sashikomi);
                        }
                    }
                }
                if (this.config.isRenhou && this.config.options.renhouAsYakuman)
                {
                    yakusInHand.Add(this.config.yaku.renhouYakuman);
                }
                if (this.config.isTenhou)
                {
                    yakusInHand.Add(this.config.yaku.tenhou);
                }
                if (this.config.isChiihou)
                {
                    yakusInHand.Add(this.config.yaku.chiihou);
                }
                if (this.config.paarenchan > 0)
                {
                    this.config.yaku.paarenchan.setPaarenchanCount(this.config.paarenchan);
                    yakusInHand.Add(this.config.yaku.paarenchan);
                }
                // calculate han
                han = 0;
                foreach (var item in yakusInHand)
                {
                    if (isOpenHand && item.nbHanOpen>0)
                    {
                        han += item.nbHanOpen;
                    }
                    else
                    {
                        han += item.nbHanClosed;
                    }
                }
                fu = 0;
                if (isAotenjou)
                {
                    if (this.config.isTsumo)
                    {
                        fu = 30;
                    }
                    else
                    {
                        fu = 40;
                    }
                    tilesForDora = tiles.ToArray().ToList();
                    doraCount = 0;
                    akaDoraCount = 0;
                    foreach (var tile in tilesForDora)
                    {
                        doraCount += U.PlusDora(tile, doraIndicators);
                    }
                    foreach (var tile in tilesForDora)
                    {
                        if (U.IsAkaDora(tile, this.config.options.hasAkaDora))
                        {
                            akaDoraCount += 1;
                        }
                    }
                    if (doraCount>0)
                    {
                        this.config.yaku.dora.nbHanOpen = doraCount;
                        this.config.yaku.dora.nbHanClosed = doraCount;
                        yakusInHand.Add(this.config.yaku.dora);
                        han += doraCount;
                    }
                    if (akaDoraCount>0)
                    {
                        this.config.yaku.akaDora.nbHanOpen = akaDoraCount;
                        this.config.yaku.akaDora.nbHanClosed = akaDoraCount;
                        yakusInHand.Add(this.config.yaku.akaDora);
                        han += akaDoraCount;
                    }
                }
                cost = scoresCalculator.CalculateScores(han, fu, this.config, yakusInHand.Count > 0);
                calculatedHands.Add(new Dictionary<string, object> {
                    {
                        "cost",
                        cost},
                    {
                        "error",
                        null},
                    {
                        "hand_yaku",
                        yakusInHand},
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
            if (!(calculatedHands.Count >0))
            {
                return new HandResponse(error: HandCalculator.ERR_HAND_NOT_CORRECT);
            }
            // let's use cost for most expensive hand
            calculatedHands = calculatedHands.OrderByDescending(x => (x["han"], x["fu"])).ToList();
            calculatedHand = calculatedHands[0];
            cost = (Dictionary<string,int>)calculatedHand["cost"];
            error = (string)calculatedHand["error"];
            yakusInHand = (List<Yaku>)calculatedHand["hand_yaku"];
            han = (int)calculatedHand["han"];
            fu = (int)calculatedHand["fu"];
            handShape = (List<List<int>>)calculatedHand["hand_shape"];
            fuDetails = (List<(int, string)>)calculatedHand["fu_details"];
            return new HandResponse(cost, han, fu, yakusInHand, handShape, error, fuDetails, isOpenHand);
        }

        /// <summary>
        /// Returns the possible win groups in 34 format
        /// </summary>
        /// <param name="tiles">14 tiles list of 136 format</param>
        /// <param name="win_tile">Winning tile, 136 format</param>
        /// <param name="melds">The list of the melds</param>
        /// <param name="dora_indicators">List of 136 format tiles</param>
        /// <param name="config">Current hand config</param>
        /// <param name="scores_calculator">Score calculator object</param>
        /// <param name="use_hand_divider_cache">True: Do not compute the same hand twice</param>
        /// <returns>A HandResponse object</returns>
        List<List<int>> FindWinGroups(int win_tile, List<List<int>> hand, List<List<int>> openedMelds34)
        {
            var winTile34 = win_tile / 4;
            // to detect win groups
            // we had to use only closed sets
            var closedSetItems = new List<List<int>>();
            foreach (var x in hand)
            {
                if (openedMelds34.Contains(x))
                {
                    closedSetItems.Remove(x);
                }
                else
                {
                    openedMelds34.Add(x);
                }
            }
            // for forms like 45666 and ron on 6
            // we can assume that ron was on 456 form and on 66 form
            // and depends on form we will have different hand cost
            // so, we had to check all possible win groups
            var winGroups = (from x in closedSetItems
                                where x.Contains(winTile34)
                                select x).ToList();
            winGroups.Distinct(new GroupComparer<int>());
            return winGroups;
        }
    }
    
}
