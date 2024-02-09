


using System.Text.Json.Serialization;

public class RankResponse
{
    [JsonPropertyName("data")]
    public RankUserResponse[] Users { get; set; }
}
public class RankUserResponse
{
    [JsonPropertyName("userID")]
    public int UserId { get; set; }
    
    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("totalScore")]
    public int Score { get; set; }
}