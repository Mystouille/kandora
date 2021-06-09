using System;
using System.Collections.Generic;

namespace kandora.bot.models
{
    public enum GameType
    {
        Mahjsoul,
        Tenhou,
        IRL,
    }
    public partial class Game
    {

        public Game(Server server)
        {
            this.Server = server;
        }

        public Game(string logId, Server server, string user1Id, string user2Id, string user3Id, string user4Id, GameType platform, DateTime timestamp) : this(server)
        {
            Id = logId;
            User1Id = user1Id;
            User2Id = user2Id;
            User3Id = user3Id;
            User4Id = user4Id;
            Platform = platform;
            Timestamp = timestamp;
        }
        public string Id { get; set; }
        public Server Server { get; set; }
        public GameType Platform { get; set; }
        public string User1Id { get; set; }
        public int? User1Score { get; set; }
        public string User2Id { get; set; }
        public int? User2Score { get; set; }
        public string User3Id { get; set; }
        public int? User3Score { get; set; }
        public string User4Id { get; set; }
        public int? User4Score { get; set; }
        public DateTime Timestamp { get; set; }
        public string FullLog { get; set; }

        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual User User3 { get; set; }
        public virtual User User4 { get; set; }

    }
}
