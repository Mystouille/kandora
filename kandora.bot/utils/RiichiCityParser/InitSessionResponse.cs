using System.Text.Json.Serialization;

public class InitSessionResponse
{
    [JsonPropertyName("data")]
    public string SID { get; set; }
}
