
namespace kandora.bot.mahjong.handcalc
{
    using C = kandora.bot.mahjong.Constants;

    public class HandConstants
    {
        public const int KAZOE_LIMITED = 0;
        public const int KAZOE_SANBAIMAN = 1;
        public const int KAZOE_NO_LIMIT = 2;
    }

    // 
    //     All the supported optional rules
    //     
    public class OptionalRules
    {

        public bool has_open_tanyao = false;
        public bool has_aka_dora = false;
        public bool has_double_yakuman = true;
        public int kazoe_limit = HandConstants.KAZOE_LIMITED;
        public bool kiriage = false;
        public bool fu_for_open_pinfu = true;
        public bool fu_for_pinfu_tsumo = false;
        public bool renhou_as_yakuman = false;
        public bool has_daisharin = false;
        public bool has_daisharin_other_suits = false;
        public bool has_daichisei = false;
        public bool has_sashikomi_yakuman = false;
        public bool limit_to_sextuple_yakuman = true;
        public bool paarenchan_needs_yaku = true;

        public OptionalRules(
            bool has_open_tanyao = false,
            bool has_aka_dora = false,
            bool has_double_yakuman = true,
            int kazoe_limit = HandConstants.KAZOE_LIMITED,
            bool kiriage = false,
            bool fu_for_open_pinfu = true,
            bool fu_for_pinfu_tsumo = false,
            bool renhou_as_yakuman = false,
            bool has_daisharin = false,
            bool has_daisharin_other_suits = false,
            bool has_sashikomi_yakuman = false,
            bool limit_to_sextuple_yakuman = true,
            bool paarenchan_needs_yaku = true,
            bool has_daichisei = false)
        {
            this.has_open_tanyao = has_open_tanyao;
            this.has_aka_dora = has_aka_dora;
            this.has_double_yakuman = has_double_yakuman;
            this.kazoe_limit = kazoe_limit;
            this.kiriage = kiriage;
            this.fu_for_open_pinfu = fu_for_open_pinfu;
            this.fu_for_pinfu_tsumo = fu_for_pinfu_tsumo;
            this.renhou_as_yakuman = renhou_as_yakuman;
            this.has_daisharin = has_daisharin || has_daisharin_other_suits;
            this.has_daisharin_other_suits = has_daisharin_other_suits;
            this.has_sashikomi_yakuman = has_sashikomi_yakuman;
            this.limit_to_sextuple_yakuman = limit_to_sextuple_yakuman;
            this.has_daichisei = has_daichisei;
            this.paarenchan_needs_yaku = paarenchan_needs_yaku;
        }
    }

    // 
    //     Special class to pass various settings to the hand calculator object
    //     
    public class HandConfig
        : HandConstants
    {

        public YakuConfig yaku = null;
        public OptionalRules options = null;
        public bool is_tsumo = false;
        public bool is_riichi = false;
        public bool is_ippatsu = false;
        public bool is_rinshan = false;
        public bool is_chankan = false;
        public bool is_haitei = false;
        public bool is_houtei = false;
        public bool is_daburu_riichi = false;
        public bool is_nagashi_mangan = false;
        public bool is_tenhou = false;
        public bool is_renhou = false;
        public bool is_chiihou = false;
        public bool is_open_riichi = false;
        public bool is_dealer = false;
        public int player_wind = 0;
        public int round_wind = 0;
        public int paarenchan = 0;
        public int kyoutaku_number = 0;
        public int tsumi_number = 0;

        public HandConfig(
            bool is_tsumo = false,
            bool is_riichi = false,
            bool is_ippatsu = false,
            bool is_rinshan = false,
            bool is_chankan = false,
            bool is_haitei = false,
            bool is_houtei = false,
            bool is_daburu_riichi = false,
            bool is_nagashi_mangan = false,
            bool is_tenhou = false,
            bool is_renhou = false,
            bool is_chiihou = false,
            bool is_open_riichi = false,
            int player_wind = 0,
            int round_wind = 0,
            int kyoutaku_number = 0,
            int tsumi_number = 0,
            int paarenchan = 0,
            OptionalRules options = null)
        {
            this.yaku = new YakuConfig();
            this.options = options == null ? new OptionalRules() : options;
            this.is_tsumo = is_tsumo;
            this.is_riichi = is_riichi;
            this.is_ippatsu = is_ippatsu;
            this.is_rinshan = is_rinshan;
            this.is_chankan = is_chankan;
            this.is_haitei = is_haitei;
            this.is_houtei = is_houtei;
            this.is_daburu_riichi = is_daburu_riichi;
            this.is_nagashi_mangan = is_nagashi_mangan;
            this.is_tenhou = is_tenhou;
            this.is_renhou = is_renhou;
            this.is_chiihou = is_chiihou;
            this.is_open_riichi = is_open_riichi;
            this.player_wind = player_wind;
            this.round_wind = round_wind;
            this.is_dealer = player_wind == C.EAST;
            this.paarenchan = paarenchan;
            this.kyoutaku_number = kyoutaku_number;
            this.tsumi_number = tsumi_number;
        }
    }
    
}
