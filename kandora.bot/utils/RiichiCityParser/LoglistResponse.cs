using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace kandora.bot.utils.RiichiCityParser
{
    public class LogListResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public LogListData[] Data { get; set; }
    }

    public class LogListData
    {
        [JsonPropertyName("paiPuId")]
        public string LogId { get; set; }

        [JsonPropertyName("isClear")]
        public bool IsIgnored { get; set; }

        [JsonPropertyName("players")]
        public PlayerData[] PlayerList { get; set; }

        public bool HasPlayer(int playerId)
        {
            return PlayerList.Select(p=>p.UserId).Contains(playerId);
        }
    }

    public class PlayerData
    {
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }
    }
}
