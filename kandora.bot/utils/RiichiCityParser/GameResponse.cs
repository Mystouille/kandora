using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GameResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("data")]
    public  GameData Data { get; set; }
}
public class GameData
{
    [JsonPropertyName("handRecord")]
    public RoundData[] Rounds { get; set; }

    [JsonPropertyName("nowTime")]
    public long RawNowTime { get; set; }
    public DateTime NowTime { get{
        return TimeUtils.GetDateTimeFromUnixMs(RawNowTime*1000);
    }}
}

public enum YakuType
{
    Riichi = 0,
    Tsumo = 1,
    Ippatsu = 2,
    Rinshan = 3,
    Haitei = 4,
    Houtei = 5,
    Chankan = 6,
    Chun = 7,
    Hatsu = 8,
    Haku = 9,
    RoundWind = 10,
    SeatWind = 11,
    Iipeikou = 12,
    Pinfu = 13,
    Tanyao = 14,
    DoubleRiichi = 15,
    Toitoi = 16,
    Chiitoitsu = 17,
    Sanankou = 18,
    Sankantsu = 19,
    Honroutou = 20,
    Chanta = 21,
    Ittsu = 22,
    SanshokuDoujoun = 23,
    Shousangen = 24,
    SanshokuDoukou = 25,
    Junchan = 26,
    Honitsu = 27,
    Ryanpeikou = 28,
    Chinitsu = 29,
    NagashiMangan = 30,
    Tenhou = 31,
    Chiihou = 32,
    Renhou = 33,
    Kokushi = 34,
    Kokushi13 = 35,
    Chuuren = 36,
    Chuuren9 = 37,
    Suuankou = 38,
    SuuankouTanki = 39,
    Suukantsu = 40,
    Chinroutou = 41,
    Tsuuiisou = 42,
    Daisuushii = 43,
    Shousuushii = 44,
    Daisangen = 45,
    Ryuuiisou = 46,
    Chinryuusou = 47,
    Paarenchan = 48,
    Aka = 49,
    Dora = 50,
    Ura = 51,
    OpenRiichi = 52,
    OpenDoubleRiichi = 53,
    OpenRiichiDealIn = 54,
    NukiDora = 55,
}

public static class Yaku
{
    public static string GetName(YakuType type)
    {
        return UsedNaming[(int)type];
    }

    public static List<string> Romaji = new List<string>(){
        "Riichi", "Tsumo", "Ippatsu", "Rinshan Kaihou", "Haitei Raoyue", "Houtei Raoyui", "Chankan", "Chun", "Hatsu", "Haku", "Round Wind",
        "Seat Wind", "Iipeikou", "Pinfu", "Tanyao", "Double Riichi", "Toitoi", "Chiitoitsu", "Sanankou", "Sankantsu", "Honroutou",
        "Chantaiyao", "Ittsu", "Sanshoku Doujun", "Shousangen", "Sanshoku Doukou", "Junchan Taiyao", "Honitsu", "Ryanpeikou", "Chinitsu", "Nagashi Mangan",
        "Tenhou", "Chiihou", "Renhou", "Kokushi Musou", "Kokushi Juusanmen", "Chuuren Poutou", "Chuuren Kyuumen", "Suuankou", "Suuankou Tanki", "Suukantsu",
        "Chinroutou", "Tsuuiisou", "Daisuushii", "Shousuushii", "Daisangen", "Ryuuiisou", "Chinryuusou", "Paarenchan", "Aka Dora", "Dora",
        "Ura Dora", "Open Riichi", "Open Double Riichi", "Open Riichi Deal-in", "Nuki Dora"
    };


    public static List<string> English = new List<string>(){
        "Riichi", "Tsumo", "Ippatsu", "After a Kan", "Under the Sea", "Under the River", "Robbing a Kan", "Red Dragon", "Green Dragon", "White Dragon", "Prevalent Wind",
        "Seat Wind", "Pure Double Sequence", "Pinfu", "All Simples", "Double Riichi", "All Triplets", "Seven Pairs", "Three Concealed Triplets", "Three Quads", "All Terminals and Honors",
        "Half Outside Hand", "Pure Straight", "Mixed Triple Sequence", "Little Three Dragons", "Triple Triplets", "Fully Outside Hand", "Half Flush", "Twice Pure Double Sequence", "Full Flush", "Mangan at Draw",
        "Blessing of Heaven", "Blessing of Earth", "Blessing of Man", "Thirteen Orphans", "Thirteen-Wait Thirteen Orphans", "Nine Gates", "True Nine Gates", "Four Concealed Triplets", "Four Concealed Triplets Single-Wait", "Four Quads",
        "All Terminals", "All Honors", "Big Four Winds", "Small Four Winds", "Big Three Dragons", "All Green", "Pure all green", "Eight consecutive dealerships", "Red Dora", "Dora",
        "Reverse Dora", "Open Riichi", "Open Double Riichi", "Open Riichi Deal-in", "Nuki Dora"
    };
    public static List<string> Japanese = new List<string>(){
        "立直", "門前清自摸和", "一発", "嶺上開花", "海底撈月", "河底撈魚", "搶槓", "役牌", "役牌", "役牌", "役牌",
        "役牌", "一盃口", "平和", "断幺九", "両立直", "対々", "七対子", "三暗刻", "三槓子", "混老頭",
        "全帯幺九", "一気通貫", "三色同順", "小三元", "三色同刻", "純全帯么", "混一色", "二盃口", "清一色", "流し満貫",
        "天和", "地和", "人和", "国士無双", "国士無双１３面待ち", "九連宝燈", "純正九蓮宝燈", "四暗刻", "四暗刻単騎", "四槓子",
        "清老頭", "字一色", "大四喜", "小四喜", "大三元", "緑一色", "純正緑一色", "八連荘", "赤ドラ", "ドラ",
        "裏ドラ", "開立直", "開両立直", "開立直", "キタ"
    };

    public static List<int> DoraList = new List<int>()
    {
        49,50,51,55
    };

    public static List<int> YakumanCount = new List<int>()
    {
        0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,
        1,1,1,1,2,1,2,1,2,1, //using renhou as yakuman here, feel free to change this to reflect EMA rules, renhou is the third value in this row
        1,1,2,1,1,1,2,1,0,0,
        0,0,0,1,0
    };

    public static List<string> UsedNaming = Romaji; // change at build (might make it dynamic but oh well)
}

public static class RCHand
{
/** Tile value:
[1-9] = pin
[17-25] = sou
[33-41] = man
49 = ton
65 = nan
81 = sha
97 = pei
113 = haku
129 = hatsu
145 = chun
261 = 0p
277 = 0s
293 = 0m
*/
    public static string? GetTileName(int? tileValue)
    {
        if(tileValue == null || tileValue == 0)
        {
            return null;
        }
        int value = (int)tileValue;
        var suits = new List<char>() {'p', 's', 'm'};
        var isAka = value > 255;
        var num = value % 16;
        var suit = ((value - num) / 16) % 16 + 1;

        return suit < 4 ? 
                isAka 
                    ? $"0{suits[suit - 1]}" 
                    : $"{num}{suits[suit - 1]}" 
                : $"{suit-3}z";
        
    }
}


public class RoundData
{
    [JsonPropertyName("changCi")]
    public int RoundNumber { get; set; }

    [JsonPropertyName("benChangNum")]
    public int Counters { get; set; }

    [JsonPropertyName("handCardEncode")]
    public string InitialWall{ get; set; }

    [JsonPropertyName("handEventRecord")]
    public HandData[] Hands { get; set; }
}


public class HandData
{
    [JsonPropertyName("eventType")]
    public EventType EventType { get; set; }

    [JsonPropertyName("data")]
    public string RawData { get; set; }

    [JsonPropertyName("userId")]
    public int userId { get; set; }

    [JsonPropertyName("startTime")]
    public long RawTime { get; set; }
    public DateTime Time { get{
        return TimeUtils.GetDateTimeFromUnixMs(RawTime);
    }}

    public SubHandData Data { get { return JsonSerializer.Deserialize<SubHandData>(RawData); } }
}


public enum EventType
{
    StartingHand = 1,
    Draw = 2,
    ActionOnDiscard = 3,
    DiscardOrCall = 4,
    RoundEnd = 5,
    GameEnd = 6,
    NewDoraIndicator = 7,
    UnknownEventType8 = 8,
    UnknownEventType9 = 9,
    UnknownEventType10 = 9,
    TenpaiReached = 11
}

public enum ActionType
{
    ChiiYXX = 2,
    ChiiXYX = 3,
    ChiiXYY = 4,
    Pon = 5,
    Ron = 7,
    Ankan = 8,
    Minkan = 9,
    Tsumo = 10,
    Discard = 11,
    Kita = 13,
}

public enum DiscardType {
    Default = 0,
    Riichi = 1,
    DoubleRiichi = 2
}

public enum RoundEndType {
    Ron= 0,
    Tsumo = 1,
    UnknownEndValue2 = 2,
    UnknownEndValue3 = 3,
    UnknownEndValue4 = 4,
    UnknownEndValue5 = 5,
    UnknownEndValue6 = 6,
    RyuuKyoku = 7,
}

public class SubHandData
{
    //======== Start of round (first 4 elements)
    [JsonPropertyName("hand_cards")]
    public int[]? RawStartingHand { get; set; }

    public string[]? StartingHand { get {
        if (RawStartingHand == null) return null;
        return (from t in RawStartingHand
               select RCHand.GetTileName(t)).ToArray();    
        } }

    [JsonPropertyName("bao_pai_card")]
    public int? Dora { get; set; }

    [JsonPropertyName("quan_feng")]
    public int? TableWind { get; set; }

    [JsonPropertyName("chang_ci")]
    public int? RoundNumber { get; set; }
    
    [JsonPropertyName("ben_chang_num")]
    public int? Counters { get; set; }
    
    [JsonPropertyName("li_zhi_bang_num")]
    public int? RiichiSticks { get; set; }

    //======== Round proceedings 

    //==== Common props

    [JsonPropertyName("action")]
    public ActionType? Action { get; set; }

    //==== Draw tile
    [JsonPropertyName("in_card")]
    public int? RawTileDrawn { get; set; }
    public string? TileDrawn
    {
        get
        {
            if (RawTileDrawn == null) return null;
            return RCHand.GetTileName(RawTileDrawn);
        }
    }

    [JsonPropertyName("is_can_lizhi")]
    public bool? CanRiichi { get; set; }

    [JsonPropertyName("is_zi_mo")]
    public bool? CanTsumo { get; set; }

    [JsonPropertyName("bu_gang_cards")]
    public int[]? RawKanCandidates { get; set; }
    public string[]? KanCandidates
    {
        get
        {
            if (RawKanCandidates == null) return null;
            return (from t in RawKanCandidates
                    select RCHand.GetTileName(t)).ToArray();
        }
    }

    [JsonPropertyName("is_gang_incard")]
    public bool? IsReplacementTile { get; set; }

    //Discard
    [JsonPropertyName("is_li_zhi")]
    public bool? DeclaresRiichi { get; set; }

    [JsonPropertyName("li_zhi_type")]
    public DiscardType? RiichiType { get; set; }


    //==== Dora revealed
    [JsonPropertyName("cards")]
    public int[]? NewDoras { get; set; }
    public string[]? NewDorasName { get {
            if(NewDoras == null) return null;
            var doras = from d in NewDoras.ToList()
                        select RCHand.GetTileName(d);
            return doras.ToArray();
        } }

    //==== Win
    [JsonPropertyName("end_type")]
    public int? EndType { get; set; }

    [JsonPropertyName("win_info")]
    public WinInfoData[] WinInfos { get; set; }

    [JsonPropertyName("user_profit")]
    public GainsData[] Gains { get; set; }

    //==== Game end
    [JsonPropertyName("user_data")]
    public GameEndData[] GameEndDataList { get; set; }

}


public class WinInfoData
{
    [JsonPropertyName("fang_info")]
    public YakuData[]? YakuList { get; set; }

    [JsonPropertyName("all_fang_num")]
    public int TotalHanValue { get; set; }

    [JsonPropertyName("all_fu")]
    public int TotalFuValue { get; set; }

    [JsonPropertyName("all_point")]
    public int TotalPointsValue { get; set; }

    [JsonPropertyName("li_bao_card")]
    public int[]? RawUraDoras { get; set; }
    public string[]? UraDoras
    {
        get
        {
            if (RawUraDoras == null) return null;
            var doras = from d in RawUraDoras.ToList()
                        select RCHand.GetTileName(d);
            return doras.ToArray();
        }
    }

    [JsonPropertyName("user_id")]
    public int Winner { get; set; }

    public int TotalDora { get {
            if (YakuList == null) return 0;
            var doras = from y in YakuList.ToList()
                        where Yaku.DoraList.Contains((int)y.YakuType)
                        select y.HanValue;
            return doras.Sum();
        } 
    }
}
public class YakuData
{
    [JsonPropertyName("fang_type")]
    public YakuType YakuType { get; set; }

    [JsonPropertyName("fang_num")]
    public int HanValue { get; set; }

    public string YakuName { get { return Yaku.GetName(YakuType); } }
}

public class GainsData
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("point_profit")]
    public int PointsGained { get; set; } // can be negative if lost

    [JsonPropertyName("li_zhi_profit")]
    public int RiichiSticksGains { get; set; }

    [JsonPropertyName("is_bao_pai")]
    public bool IsPao { get; set; }

    [JsonPropertyName("user_point")]
    public int ScoreAfterPayment { get; set; }
}

public class GameEndData
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("point_num")]
    public int FinalPoints { get; set; } // can be negative

    [JsonPropertyName("score")]
    public int FinalScore { get; set; } //After oka/uma
}
