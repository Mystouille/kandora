using System.Text.Json.Serialization;

public class LoginResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public LoginResponseData Data { get; set; }
}


public class LoginResponseData
{
    [JsonPropertyName("user")]
    public LoginResponseUser User { get; set; }
}

public class LoginResponseUser
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
}

