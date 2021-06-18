
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

        public bool hasOpenTanyao = false;
        public bool hasAkaDora = false;
        public bool hasDoubleYakuman = true;
        public int kazoeLimit = HandConstants.KAZOE_LIMITED;
        public bool kiriage = false;
        public bool fuForOpenPinfu = true;
        public bool fuForPinfuTsumo = false;
        public bool renhouAsYakuman = false;
        public bool hasDaisharin = false;
        public bool hasDaisharinOtherSuits = false;
        public bool hasSashikomiYakuman = false;
        public bool limitToSextupleYakuman = true;
        public bool paarenchanNeedsYaku = true;
        public bool hasDaichisei = false;

        public OptionalRules(
            bool hasOpenTanyao = false,
            bool hasAkaDora = false,
            bool hasDoubleYakuman = true,
            int kazoeLimit = HandConstants.KAZOE_LIMITED,
            bool kiriage = false,
            bool fuForOpenPinfu = true,
            bool fuForPinfuTsumo = false,
            bool renhouAsYakuman = false,
            bool hasDaisharin = false,
            bool hasDaisharinOtherSuits = false,
            bool hasSashikomiYakuman = false,
            bool limitToSextupleYakuman = true,
            bool paarenchanNeedsYaku = true,
            bool hasDaichisei = false)
        {
            this.hasOpenTanyao = hasOpenTanyao;
            this.hasAkaDora = hasAkaDora;
            this.hasDoubleYakuman = hasDoubleYakuman;
            this.kazoeLimit = kazoeLimit;
            this.kiriage = kiriage;
            this.fuForOpenPinfu = fuForOpenPinfu;
            this.fuForPinfuTsumo = fuForPinfuTsumo;
            this.renhouAsYakuman = renhouAsYakuman;
            this.hasDaisharin = hasDaisharin || hasDaisharinOtherSuits;
            this.hasDaisharinOtherSuits = hasDaisharinOtherSuits;
            this.hasSashikomiYakuman = hasSashikomiYakuman;
            this.limitToSextupleYakuman = limitToSextupleYakuman;
            this.hasDaichisei = hasDaichisei;
            this.paarenchanNeedsYaku = paarenchanNeedsYaku;
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
        public bool isTsumo = false;
        public bool isRiichi = false;
        public bool isIppatsu = false;
        public bool isRinshan = false;
        public bool isChankan = false;
        public bool isHaitei = false;
        public bool isHoutei = false;
        public bool isDaburuRiichi = false;
        public bool isNagashiMangan = false;
        public bool isTenhou = false;
        public bool isRenhou = false;
        public bool isChiihou = false;
        public bool isOpenRiichi = false;
        public bool isDealer = false;
        public int playerWind = 0;
        public int roundWind = 0;
        public int kyoutakuNumber = 0;
        public int tsumiNumber = 0;
        public int paarenchan = 0;

        public HandConfig(
            bool isTsumo = false,
            bool isRiichi = false,
            bool isIppatsu = false,
            bool isRinshan = false,
            bool isChankan = false,
            bool isHaitei = false,
            bool isHoutei = false,
            bool isDaburuRiichi = false,
            bool isNagashiMangan = false,
            bool isTenhou = false,
            bool isRenhou = false,
            bool isChiihou = false,
            bool isOpenRiichi = false,
            int playerWind = 0,
            int roundWind = 0,
            int kyoutakuNumber = 0,
            int tsumiNumber = 0,
            int paarenchan = 0,
            OptionalRules options = null)
        {
            this.yaku = new YakuConfig();
            this.options = options == null ? new OptionalRules() : options;
            this.isTsumo = isTsumo;
            this.isRiichi = isRiichi;
            this.isIppatsu = isIppatsu;
            this.isRinshan = isRinshan;
            this.isChankan = isChankan;
            this.isHaitei = isHaitei;
            this.isHoutei = isHoutei;
            this.isDaburuRiichi = isDaburuRiichi;
            this.isNagashiMangan = isNagashiMangan;
            this.isTenhou = isTenhou;
            this.isRenhou = isRenhou;
            this.isChiihou = isChiihou;
            this.isOpenRiichi = isOpenRiichi;
            this.playerWind = playerWind;
            this.roundWind = roundWind;
            this.isDealer = playerWind == C.EAST;
            this.paarenchan = paarenchan;
            this.kyoutakuNumber = kyoutakuNumber;
            this.tsumiNumber = tsumiNumber;
        }
    }
    
}
