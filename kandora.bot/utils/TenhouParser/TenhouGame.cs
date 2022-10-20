using kandora.bot.http;
using System.Text.Json.Serialization;

namespace kandora.bot.utils.TenhouParser;

public class TenhouGame
{
    [JsonPropertyName("ver")]
    public string Version { get; set; }

    [JsonPropertyName("ref")]
    public string Reference { get; set; }

    [JsonPropertyName("rule")]
    public TenhouRule Rule { get; set; }

    [JsonPropertyName("dan")]
    public string[] Dans { get; set; }

    [JsonPropertyName("sc")]
    public float[] Scores { get; set; }

    [JsonPropertyName("name")]
    public string[] Names { get; set; }

    [JsonPropertyName("mjsoulId")]
    public int[] MahjongSoulIds { get; set; }

    [JsonPropertyName("rate")]
    public float[] Rate { get; set; }

    [JsonPropertyName("lobby")]
    public int Lobby { get; set; }

    [JsonPropertyName("sx")]
    public string[] Sx { get; set; }

    [JsonPropertyName("ratingc")]
    public string Rating { get; set; }

    [JsonPropertyName("title")]
    public string[] Title { get; set; }

    [JsonPropertyName("log")]
    public object[][][] Logs { get; set; }

    public string[] GetIds()
    {
        if(MahjongSoulIds != null)
        {
            return MahjongSoulIds.ToStringArray();
        }
        return new string[Names.Length];
    }
}

public class TenhouRule
{
    [JsonPropertyName("disp")]
    public string Disp { get; set; }
    [JsonPropertyName("aka53")]
    public int Aka53 { get; set; }
    [JsonPropertyName("aka52")]
    public int Aka52 { get; set; }
    [JsonPropertyName("aka51")]
    public int Aka51 { get; set; }

    public Rule ToRiichiRule()
    {
        return new Rule()
        {
            Disp = Disp,
            Aka51 = Aka51,
            Aka52 = Aka52,
            Aka53 = Aka53
        };
    }
}
