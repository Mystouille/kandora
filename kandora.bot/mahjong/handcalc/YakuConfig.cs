using kandora.bot.mahjong.handcalc.yaku;
using kandora.bot.mahjong.handcalc.yaku.yakuman;

namespace kandora.bot.mahjong.handcalc
{
    public class YakuConfig
    {
        public Tsumo tsumo;
        public Riichi riichi;
        public OpenRiichi open_riichi;
        public Ippatsu ippatsu;
        public Chankan chankan;
        public Rinshan rinshan;
        public Haitei haitei;
        public Houtei houtei;
        public DaburuRiichi daburu_riichi;
        public DaburuOpenRiichi daburu_open_riichi;
        public NagashiMangan nagashi_mangan;
        public Renhou renhou;
        public Pinfu pinfu;
        public Tanyao tanyao;
        public Iipeikou iipeiko;
        public Haku haku;
        public Hatsu hatsu;
        public Chun chun;
        public East east;
        public South south;
        public West west;
        public North north;
        public YakuhaiPlace yakuhai_place;
        public YakuhaiRound yakuhai_round;
        public Sanshoku sanshoku;
        public Ittsu ittsu;
        public Chanta chantai;
        public Honroutou honroto;
        public Toitoi toitoi;
        public Sanankou sanankou;
        public Sankantsu sankantsu;
        public SanshokuDoukou sanshoku_douko;
        public Chiitoitsu chiitoitsu;
        public Shousangen shosangen;
        public Honitsu honitsu;
        public Junchan junchan;
        public Ryanpeikou ryanpeiko;
        public Chinitsu chinitsu;
        public Kokushi kokushi;
        public ChuurenPoutou chuuren_poutou;
        public Suuankou suuankou;
        public Daisangen daisangen;
        public Shousuushii shosuushi;
        public Ryuuiisou ryuisou;
        public Suukantsu suukantsu;
        public Tsuisou tsuisou;
        public Chinroutou chinroto;
        public Daisharin daisharin;
        public Daichisei daichisei;
        public Daisuushii daisuushi;
        public DaburuKokushi daburu_kokushi;
        public SuuankouTanki suuankou_tanki;
        public DaburuChuurenPoutou daburu_chuuren_poutou;
        public Tenhou tenhou;
        public Chiihou chiihou;
        public RenhouYakuman renhou_yakuman;
        public Sashikomi sashikomi;
        public Paarenchan paarenchan;
        // Other
        public Dora dora;
        public AkaDora aka_dora;

        public YakuConfig()
        {
            var id = 0;
            aka_dora = new AkaDora(++id);
            tenhou = new Tenhou(++id);
            // Yaku situations
            tsumo = new Tsumo(++id);
            riichi = new Riichi(++id);
            open_riichi = new OpenRiichi(++id);
            ippatsu = new Ippatsu(++id);
            chankan = new Chankan(++id);
            rinshan = new Rinshan(++id);
            haitei = new Haitei(++id);
            houtei = new Houtei(++id);
            daburu_riichi = new DaburuRiichi(++id);
            daburu_open_riichi = new DaburuOpenRiichi(++id);
            nagashi_mangan = new NagashiMangan(++id);
            renhou = new Renhou(++id);
            // Yaku 1 Han
            pinfu = new Pinfu(++id);
            tanyao = new Tanyao(++id);
            iipeiko = new Iipeikou(++id);
            haku = new Haku(++id);
            hatsu = new Hatsu(++id);
            chun = new Chun(++id);
            east = new East(++id);
            south = new South(++id);
            west = new West(++id);
            north = new North(++id);
            yakuhai_place = new YakuhaiPlace(++id);
            yakuhai_round = new YakuhaiRound(++id);
            // Yaku 2 Hans
            sanshoku = new Sanshoku(++id);
            ittsu = new Ittsu(++id);
            chantai = new Chanta(++id);
            honroto = new Honroutou(++id);
            toitoi = new Toitoi(++id);
            sanankou = new Sanankou(++id);
            sankantsu = new Sankantsu(++id);
            sanshoku_douko = new SanshokuDoukou(++id);
            chiitoitsu = new Chiitoitsu(++id);
            shosangen = new Shousangen(++id);
            // Yaku 3 Hans
            honitsu = new Honitsu(++id);
            junchan = new Junchan(++id);
            ryanpeiko = new Ryanpeikou(++id);
            // Yaku 6 Hans
            chinitsu = new Chinitsu(++id);
            // Yakuman list
            kokushi = new Kokushi(++id);
            chuuren_poutou = new ChuurenPoutou(++id);
            suuankou = new Suuankou(++id);
            daisangen = new Daisangen(++id);
            shosuushi = new Shousuushii(++id);
            ryuisou = new Ryuuiisou(++id);
            suukantsu = new Suukantsu(++id);
            tsuisou = new Tsuisou(++id);
            chinroto = new Chinroutou(++id);
            daisharin = new Daisharin(++id);
            daichisei = new Daichisei(++id);
            // Double yakuman
            daisuushi = new Daisuushii(++id);
            daburu_kokushi = new DaburuKokushi(++id);
            suuankou_tanki = new SuuankouTanki(++id);
            daburu_chuuren_poutou = new DaburuChuurenPoutou(++id);
            // Yakuman situations
            tenhou = new Tenhou(id++);
            chiihou = new Chiihou(++id);
            renhou_yakuman = new RenhouYakuman(++id);
            sashikomi = new Sashikomi(++id);
            paarenchan = new Paarenchan(++id);
            // Other
            dora = new Dora(++id);
            aka_dora = new AkaDora(++id);
        }
    }
    
}
