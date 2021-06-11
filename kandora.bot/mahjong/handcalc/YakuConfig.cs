using kandora.bot.mahjong.handcalc.yaku;
using kandora.bot.mahjong.handcalc.yaku.yakuman;

namespace kandora.bot.mahjong.handcalc
{
    public class YakuConfig
    {
        public Yaku tsumo;
        public Yaku riichi;
        public Yaku open_riichi;
        public Yaku ippatsu;
        public Yaku chankan;
        public Yaku rinshan;
        public Yaku haitei;
        public Yaku houtei;
        public Yaku daburu_riichi;
        public Yaku daburu_open_riichi;
        public Yaku nagashi_mangan;
        public Yaku renhou;
        public Yaku pinfu;
        public Yaku tanyao;
        public Yaku iipeiko;
        public Yaku haku;
        public Yaku hatsu;
        public Yaku chun;
        public Yaku east;
        public Yaku south;
        public Yaku west;
        public Yaku north;
        public Yaku yakuhai_place;
        public Yaku yakuhai_round;
        public Yaku sanshoku;
        public Yaku ittsu;
        public Yaku chantai;
        public Yaku honroto;
        public Yaku toitoi;
        public Yaku sanankou;
        public Yaku sankantsu;
        public Yaku sanshoku_douko;
        public Yaku chiitoitsu;
        public Yaku shosangen;
        public Yaku honitsu;
        public Yaku junchan;
        public Yaku ryanpeiko;
        public Yaku chinitsu;
        public Yaku kokushi;
        public Yaku chuuren_poutou;
        public Yaku suuankou;
        public Yaku daisangen;
        public Yaku shosuushi;
        public Yaku ryuisou;
        public Yaku suukantsu;
        public Yaku tsuisou;
        public Yaku chinroto;
        public Yaku daisharin;
        public Yaku daichisei;
        public Yaku daisuushi;
        public Yaku daburu_kokushi;
        public Yaku suuankou_tanki;
        public Yaku daburu_chuuren_poutou;
        public Yaku tenhou;
        public Yaku chiihou;
        public Yaku renhou_yakuman;
        public Yaku sashikomi;
        public Yaku paarenchan;
        // Other
        public Yaku dora;
        public Yaku aka_dora;

        public YakuConfig()
        {
            var id = 0;
            aka_dora = new AkaDora(++id);
            tenhou = new Tenhou(++id);
            // Yaku situations
            tsumo = new AkaDora(++id);
            riichi = new AkaDora(++id);
            open_riichi = new AkaDora(++id);
            ippatsu = new AkaDora(++id);
            chankan = new AkaDora(++id);
            rinshan = new AkaDora(++id);
            haitei = new AkaDora(++id);
            houtei = new AkaDora(++id);
            daburu_riichi = new AkaDora(++id);
            daburu_open_riichi = new AkaDora(++id);
            nagashi_mangan = new AkaDora(++id);
            renhou = new AkaDora(++id);
            // Yaku 1 Han
            pinfu = new AkaDora(++id);
            tanyao = new AkaDora(++id);
            iipeiko = new AkaDora(++id);
            haku = new AkaDora(++id);
            hatsu = new AkaDora(++id);
            chun = new AkaDora(++id);
            east = new AkaDora(++id);
            south = new AkaDora(++id);
            west = new AkaDora(++id);
            north = new AkaDora(++id);
            yakuhai_place = new AkaDora(++id);
            yakuhai_round = new AkaDora(++id);
            // Yaku 2 Hans
            sanshoku = new AkaDora(++id);
            ittsu = new AkaDora(++id);
            chantai = new AkaDora(++id);
            honroto = new AkaDora(++id);
            toitoi = new AkaDora(++id);
            sanankou = new AkaDora(++id);
            sankantsu = new AkaDora(++id);
            sanshoku_douko = new AkaDora(++id);
            chiitoitsu = new AkaDora(++id);
            shosangen = new AkaDora(++id);
            // Yaku 3 Hans
            honitsu = new AkaDora(++id);
            junchan = new AkaDora(++id);
            ryanpeiko = new AkaDora(++id);
            // Yaku 6 Hans
            chinitsu = new AkaDora(++id);
            // Yakuman list
            kokushi = new AkaDora(++id);
            chuuren_poutou = new AkaDora(++id);
            suuankou = new AkaDora(++id);
            daisangen = new AkaDora(++id);
            shosuushi = new AkaDora(++id);
            ryuisou = new AkaDora(++id);
            suukantsu = new AkaDora(++id);
            tsuisou = new AkaDora(++id);
            chinroto = new AkaDora(++id);
            daisharin = new AkaDora(++id);
            daichisei = new AkaDora(++id);
            // Double yakuman
            daisuushi = new AkaDora(++id);
            daburu_kokushi = new AkaDora(++id);
            suuankou_tanki = new AkaDora(++id);
            daburu_chuuren_poutou = new AkaDora(++id);
            // Yakuman situations
            tenhou = new Tenhou(id++);
            chiihou = new AkaDora(++id);
            renhou_yakuman = new AkaDora(++id);
            sashikomi = new AkaDora(++id);
            paarenchan = new AkaDora(++id);
            // Other
            dora = new AkaDora(++id);
            aka_dora = new AkaDora(++id);

            //// Yaku situations
            //this.tsumo = Tsumo(next(id));
            //this.riichi = Riichi(next(id));
            //this.open_riichi = OpenRiichi(next(id));
            //this.ippatsu = Ippatsu(next(id));
            //this.chankan = Chankan(next(id));
            //this.rinshan = Rinshan(next(id));
            //this.haitei = Haitei(next(id));
            //this.houtei = Houtei(next(id));
            //this.daburu_riichi = DaburuRiichi(next(id));
            //this.daburu_open_riichi = DaburuOpenRiichi(next(id));
            //this.nagashi_mangan = NagashiMangan(next(id));
            //this.renhou = Renhou(next(id));
            //// Yaku 1 Han
            //this.pinfu = Pinfu(next(id));
            //this.tanyao = Tanyao(next(id));
            //this.iipeiko = Iipeiko(next(id));
            //this.haku = Haku(next(id));
            //this.hatsu = Hatsu(next(id));
            //this.chun = Chun(next(id));
            //this.east = YakuhaiEast(next(id));
            //this.south = YakuhaiSouth(next(id));
            //this.west = YakuhaiWest(next(id));
            //this.north = YakuhaiNorth(next(id));
            //this.yakuhai_place = YakuhaiOfPlace(next(id));
            //this.yakuhai_round = YakuhaiOfRound(next(id));
            //// Yaku 2 Hans
            //this.sanshoku = Sanshoku(next(id));
            //this.ittsu = Ittsu(next(id));
            //this.chantai = Chantai(next(id));
            //this.honroto = Honroto(next(id));
            //this.toitoi = Toitoi(next(id));
            //this.sanankou = Sanankou(next(id));
            //this.sankantsu = SanKantsu(next(id));
            //this.sanshoku_douko = SanshokuDoukou(next(id));
            //this.chiitoitsu = Chiitoitsu(next(id));
            //this.shosangen = Shosangen(next(id));
            //// Yaku 3 Hans
            //this.honitsu = Honitsu(next(id));
            //this.junchan = Junchan(next(id));
            //this.ryanpeiko = Ryanpeikou(next(id));
            //// Yaku 6 Hans
            //this.chinitsu = Chinitsu(next(id));
            //// Yakuman list
            //this.kokushi = KokushiMusou(next(id));
            //this.chuuren_poutou = ChuurenPoutou(next(id));
            //this.suuankou = Suuankou(next(id));
            //this.daisangen = Daisangen(next(id));
            //this.shosuushi = Shousuushii(next(id));
            //this.ryuisou = Ryuuiisou(next(id));
            //this.suukantsu = Suukantsu(next(id));
            //this.tsuisou = Tsuuiisou(next(id));
            //this.chinroto = Chinroutou(next(id));
            //this.daisharin = Daisharin(next(id));
            //this.daichisei = Daichisei(next(id));
            //// Double yakuman
            //this.daisuushi = DaiSuushii(next(id));
            //this.daburu_kokushi = DaburuKokushiMusou(next(id));
            //this.suuankou_tanki = SuuankouTanki(next(id));
            //this.daburu_chuuren_poutou = DaburuChuurenPoutou(next(id));
            //// Yakuman situations
            //this.tenhou = Tenhou(next(id));
            //this.chiihou = Chiihou(next(id));
            //this.renhou_yakuman = RenhouYakuman(next(id));
            //this.sashikomi = Sashikomi(next(id));
            //this.paarenchan = Paarenchan(next(id));
            //// Other
            //this.dora = Dora(next(id));
            //this.aka_dora = AkaDora(next(id));
        }
    }
    
}
