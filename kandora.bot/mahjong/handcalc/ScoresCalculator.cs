using System;
using System.Linq;

namespace kandora.bot.mahjong.handcalc
{

    using System.Collections.Generic;

    public enum YakuLevel{
        None = 0,
        KiriageMangan = 1,
        NagashiMangan = 2,
        Mangan = 3,
        Haneman = 4,
        Baiman = 5,
        KazoeSanbaiman = 6,
        Sanbaiman = 7,
        KazoeYakuman = 8,
        Yakuman = 9,
        Yakuman2x = 10,
        Yakuman3x = 11,
        Yakuman4x = 12,
        Yakuman5x = 13,
        Yakuman6x = 14,
    }

    public class ScoresCalculator
    {

        // 
        //         Calculate how much scores cost a hand with given han and fu
        //         :param han: int
        //         :param fu: int
        //         :param config: HandConfig object
        //         :param is_yakuman: boolean
        //         :return: a dictionary with following keys:
        //         'main': main cost (honba number / tsumi bou not included)
        //         'additional': additional cost (honba number not included)
        //         'main_bonus': extra cost due to honba number to be added on main cost
        //         'additional_bonus': extra cost due to honba number to be added on additional cost
        //         'kyoutaku_bonus': the points taken from accumulated riichi 1000-point bous (kyoutaku)
        //         'total': the total points the winner is to earn
        //         'yaku_level': level of yaku (e.g. yakuman, mangan, nagashi mangan, etc)
        // 
        //         for ron, main cost is the cost for the player who triggers the ron, and additional cost is always = 0
        //         for dealer tsumo, main cost is the same as additional cost, which is the cost for any other player
        //         for non-dealer (player) tsumo, main cost is cost for dealer and additional is cost for player
        // 
        //         examples:
        //         1. dealer tsumo 2000 ALL in 2 honba, with 3 riichi bous on desk
        //         {'main': 2000, 'additional': 2000,
        //          'main_bonus': 200, 'additional_bonus': 200,
        //          'kyoutaku_bonus': 3000, 'total': 9600, 'yaku_level': ''}
        // 
        //          2. player tsumo 3900-2000 in 4 honba, with 1 riichi bou on desk
        //          {'main': 3900, 'additional': 2000,
        //          'main_bonus': 400, 'additional_bonus': 400,
        //          'kyoutaku_bonus': 1000, 'total': 10100, 'yaku_level': ''}
        // 
        //          3. dealer (or player) ron 12000 in 5 honba, with no riichi bou on desk
        //          {'main': 12000, 'additional': 0,
        //          'main_bonus': 1500, 'additional_bonus': 0,
        //          'kyoutaku_bonus': 0, 'total': 13500}
        // 
        //         
        public virtual Dictionary<string, int> CalculateScores(int han, int fu, HandConfig config, bool is_yakuman = false)
        {
            int additional;
            int additional_bonus;
            int main_bonus;
            int rounded;
            int double_rounded;
            int four_rounded;
            int six_rounded;
            int main;
            int yaku_level = (int)YakuLevel.None;
            // kazoe hand
            if (han >= 13 && !is_yakuman)
            {
                // Hands over 26+ han don't count as double yakuman
                if (config.options.kazoeLimit == HandConfig.KAZOE_LIMITED)
                {
                    han = 13;
                    yaku_level = (int)YakuLevel.KazoeYakuman;
                }
                else if (config.options.kazoeLimit == HandConfig.KAZOE_SANBAIMAN)
                {
                    // Hands over 13+ is a sanbaiman
                    han = 12;
                    yaku_level = (int)YakuLevel.KazoeSanbaiman;
                }
            }
            if (han >= 5)
            {
                if (han >= 78)
                {
                    yaku_level = (int)YakuLevel.Yakuman6x;
                    if (config.options.limitToSextupleYakuman)
                    {
                        rounded = 48000;
                    }
                    else
                    {
                        int quot = Math.DivRem(han - 78, 13, out _);
                        var extra_han = quot;
                        rounded = 48000 + extra_han * 8000;
                    }
                }
                else if (han >= 65)
                {
                    yaku_level = (int)YakuLevel.Yakuman5x;
                    rounded = 40000;
                }
                else if (han >= 52)
                {
                    yaku_level = (int)YakuLevel.Yakuman4x;
                    rounded = 32000;
                }
                else if (han >= 39)
                {
                    yaku_level = (int)YakuLevel.Yakuman3x;
                    rounded = 24000;
                }
                else if (han >= 26)
                {
                    // double yakuman
                    yaku_level = (int)YakuLevel.Yakuman2x;
                    rounded = 16000;
                }
                else if (han >= 13)
                {
                    // yakuman
                    yaku_level = (int)YakuLevel.Yakuman;
                    rounded = 8000;
                }
                else if (han >= 11)
                {
                    // sanbaiman
                    yaku_level = (int)YakuLevel.Sanbaiman;
                    rounded = 6000;
                }
                else if (han >= 8)
                {
                    // baiman
                    yaku_level = (int)YakuLevel.Baiman;
                    rounded = 4000;
                }
                else if (han >= 6)
                {
                    // haneman
                    yaku_level = (int)YakuLevel.Haneman;
                    rounded = 3000;
                }
                else
                {
                    yaku_level = (int)YakuLevel.Mangan;
                    rounded = 2000;
                }
                double_rounded = rounded * 2;
                four_rounded = double_rounded * 2;
                six_rounded = double_rounded * 3;
            }
            else
            {
                // han < 5
                int base_points = fu * (int)Math.Pow(2, 2 + han);
                rounded = (base_points + 99) / 100 * 100;
                double_rounded = (2 * base_points + 99) / 100 * 100;
                four_rounded = (4 * base_points + 99) / 100 * 100;
                six_rounded = (6 * base_points + 99) / 100 * 100;
                var is_kiriage = false;
                if (config.options.kiriage)
                {
                    if (han == 4 && fu == 30 || han == 3 && fu == 60)
                    {
                        yaku_level = (int)YakuLevel.KiriageMangan;
                        is_kiriage = true;
                    }
                }
                else
                {
                    // kiriage not supported
                    if (rounded > 2000)
                    {
                        yaku_level = (int)YakuLevel.Mangan;
                    }
                }
                // mangan
                if (rounded > 2000 || is_kiriage)
                {
                    rounded = 2000;
                    double_rounded = rounded * 2;
                    four_rounded = double_rounded * 2;
                    six_rounded = double_rounded * 3;
                }
                else
                {
                    // below mangan
                }
            }
            if (config.isTsumo)
            {
                main = double_rounded;
                main_bonus = 100 * config.tsumiNumber;
                additional_bonus = main_bonus;
                if (config.isDealer)
                {
                    additional = main;
                }
                else
                {
                    // player
                    additional = rounded;
                }
            }
            else
            {
                // ron
                additional = 0;
                additional_bonus = 0;
                main_bonus = 300 * config.tsumiNumber;
                if (config.isDealer)
                {
                    main = six_rounded;
                }
                else
                {
                    // player
                    main = four_rounded;
                }
            }
            var kyoutaku_bonus = 1000 * config.kyoutakuNumber;
            var total = main + main_bonus + 2 * (additional + additional_bonus) + kyoutaku_bonus;
            if (config.isNagashiMangan)
            {
                yaku_level = (int)YakuLevel.NagashiMangan;
            }
            var ret_dict = new Dictionary<string, int> {
                {
                    "main",
                    main},
                {
                    "main_bonus",
                    main_bonus},
                {
                    "additional",
                    additional},
                {
                    "additional_bonus",
                    additional_bonus},
                {
                    "kyoutaku_bonus",
                    kyoutaku_bonus},
                {
                    "total",
                    total},
                {
                    "yaku_level",
                    yaku_level}
            };
            return ret_dict;
        }
        public static void aotenjouFilterYaku(List<Yaku> hand_yaku, HandConfig config)
        {
            // do nothing
        }
    }

    public class Aotenjou : ScoresCalculator
    {

        public override Dictionary<string,int> CalculateScores(int han, int fu, HandConfig config, bool is_yakuman = false)
        {
            int base_points = fu * (int)Math.Pow(2, 2 + han);
            int rounded = (base_points + 99) / 100 * 100;
            int double_rounded = (2 * base_points + 99) / 100 * 100;
            int four_rounded = (4 * base_points + 99) / 100 * 100;
            int six_rounded = (6 * base_points + 99) / 100 * 100;
            if (config.isTsumo)
            {
                return new Dictionary<string, int> {
                    {
                        "main",
                        double_rounded},
                    {
                        "additional",
                        config.isDealer ? double_rounded : rounded}};
            }
            else
            {
                return new Dictionary<string, int> {
                    {
                        "main",
                        config.isDealer ? six_rounded : four_rounded},
                    {
                        "additional",
                        0}};
            }
        }

        public static void aotenjou_filter_yaku(List<Yaku> hand_yaku, HandConfig config)
        {
            // in aotenjou yakumans are normal yaku
            // but we need to filter lower yaku that are precursors to yakumans
            if (hand_yaku.Contains(config.yaku.daisangen))
            {
                // for daisangen precursors are all dragons and shosangen
                hand_yaku.Remove(config.yaku.chun);
                hand_yaku.Remove(config.yaku.hatsu);
                hand_yaku.Remove(config.yaku.haku);
                hand_yaku.Remove(config.yaku.shosangen);
            }
            if (hand_yaku.Contains(config.yaku.tsuisou))
            {
                // for tsuuiisou we need to remove toitoi and honroto
                hand_yaku.Remove(config.yaku.toitoi);
                hand_yaku.Remove(config.yaku.honroto);
            }
            if (hand_yaku.Contains(config.yaku.shosuushi))
            {
                // for shosuushi we do not need to remove anything
            }
            if (hand_yaku.Contains(config.yaku.daisuushi))
            {
                // for daisuushi we need to remove toitoi
                hand_yaku.Remove(config.yaku.toitoi);
            }
            if (hand_yaku.Contains(config.yaku.suuankou) || hand_yaku.Contains(config.yaku.suuankouTanki))
            {
                // for suu ankou we need to remove toitoi and sanankou (sanankou is already removed by default)
                if (hand_yaku.Contains(config.yaku.toitoi))
                {
                    // toitoi is "optional" in closed suukantsu, maybe a bug? or toitoi is not given when it's kans?
                    hand_yaku.Remove(config.yaku.toitoi);
                }
            }
            if (hand_yaku.Contains(config.yaku.chinroto))
            {
                // for chinroto we need to remove toitoi and honroto
                hand_yaku.Remove(config.yaku.toitoi);
                hand_yaku.Remove(config.yaku.honroto);
            }
            if (hand_yaku.Contains(config.yaku.suukantsu))
            {
                // for suukantsu we need to remove toitoi and sankantsu (sankantsu is already removed by default)
                if (hand_yaku.Contains(config.yaku.toitoi))
                {
                    // same as above?
                    hand_yaku.Remove(config.yaku.toitoi);
                }
            }
            if (hand_yaku.Contains(config.yaku.chuurenPoutou) || hand_yaku.Contains(config.yaku.daburuChuurenPoutou))
            {
                // for chuuren poutou we need to remove chinitsu
                hand_yaku.Remove(config.yaku.chinitsu);
            }
            if (hand_yaku.Contains(config.yaku.daisharin))
            {
                // for daisharin we need to remove chinitsu, pinfu, tanyao, ryanpeiko, chiitoitsu
                hand_yaku.Remove(config.yaku.chinitsu);
                if (hand_yaku.Contains(config.yaku.pinfu))
                {
                    hand_yaku.Remove(config.yaku.pinfu);
                }
                hand_yaku.Remove(config.yaku.tanyao);
                if (hand_yaku.Contains(config.yaku.ryanpeiko))
                {
                    hand_yaku.Remove(config.yaku.ryanpeiko);
                }
                if (hand_yaku.Contains(config.yaku.chiitoitsu))
                {
                    hand_yaku.Remove(config.yaku.chiitoitsu);
                }
            }
            if (hand_yaku.Contains(config.yaku.ryuisou))
            {
                // for ryuisou we need to remove honitsu, if it is there
                if (hand_yaku.Contains(config.yaku.honitsu))
                {
                    hand_yaku.Remove(config.yaku.honitsu);
                }
            }
        }
    }
    
}
