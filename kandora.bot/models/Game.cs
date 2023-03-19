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

        public Game(int id, string gameName, Server server, string user1Id, string user2Id, string user3Id, string user4Id, GameType platform, string location, DateTime timestamp, bool isSanma ) : this(server)
        {
            Id = id;
            GameName = gameName;
            User1Id = user1Id;
            User2Id = user2Id;
            User3Id = user3Id;
            User4Id = user4Id;
            Platform = platform;
            Timestamp = timestamp;
            IsSanma = isSanma;
            Location = location;
        }
        public int Id { get; set; }
        public string GameName { get; set; }
        public Server Server { get; set; }
        public GameType Platform { get; set; }
        public string Location { get; set; }
        public bool IsSanma { get; set; }   
        public string LocationStr
        {
            get
            {
                switch (this.Platform)
                {
                    case GameType.Mahjsoul:
                        return "Mahjsoul";
                    case GameType.Tenhou:
                        return "Tenhou";
                    case GameType.IRL:
                        return $"IRL: {Location}";
                    default: return "IRL";
                }
            }
        }

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

        public int User1Chombo { get; set; }
        public int User2Chombo { get; set; }
        public int User3Chombo { get; set; }
        public int User4Chombo { get; set; }
    }
}
