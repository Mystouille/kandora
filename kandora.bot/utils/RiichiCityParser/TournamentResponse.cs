


using System.Text.Json.Serialization;

public class RankResponse
{
    [JsonPropertyName("data")]
    public RankUserResponse[] Users { get; set; }
    [JsonPropertyName("code")]
    public int Code { get; set; }
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

public class PlayerStatusResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("data")]
    public PlayerStatusData[] Data { get; set; }
}
public class PlayerStatusData
{
    [JsonPropertyName("nickname")]
    public string Name { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("userID")]
    public int UserId { get; set; }

}

public class TournamentInfoResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("data")]
    public TournamentInfoData Data { get; set; }
}
public class TournamentInfoData
{
    [JsonPropertyName("classifyID")]
    public string ClassifyId { get; set; }
}

public class StartGameResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("data")]
    public bool IsSuccess { get; set; }
}
