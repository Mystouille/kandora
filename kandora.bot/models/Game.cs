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
        public string PlatformStr
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
                        return "IRL";
                    default: return "IRL";
                }
            }
        }
        public string User1Id { get; set; }
        public int? User1Score { get; set; }
        public int User1Placement { get {
                var i = 1;
                if(User1Score.HasValue && User2Score.HasValue && User1Score < User2Score)
                {
                    i++;
                }
                if (User1Score.HasValue && User3Score.HasValue && User1Score < User3Score)
                {
                    i++;
                }
                if (User1Score.HasValue && User4Score.HasValue && User1Score < User4Score)
                {
                    i++;
                }
                return i;
            } }
        public string User2Id { get; set; }
        public int? User2Score { get; set; }
        public int User2Placement
        {
            get
            {
                var i = 1;
                if (User2Score.HasValue && User1Score.HasValue && User2Score < User2Score)
                {
                    i++;
                }
                if (User2Score.HasValue && User3Score.HasValue && User2Score < User3Score)
                {
                    i++;
                }
                if (User2Score.HasValue && User4Score.HasValue && User2Score < User4Score)
                {
                    i++;
                }
                return i;
            }
        }
        public string User3Id { get; set; }
        public int? User3Score { get; set; }
        public int User3Placement
        {
            get
            {
                var i = 1;
                if (User3Score.HasValue && User2Score.HasValue && User3Score < User2Score)
                {
                    i++;
                }
                if (User3Score.HasValue && User1Score.HasValue && User3Score < User1Score)
                {
                    i++;
                }
                if (User3Score.HasValue && User4Score.HasValue && User3Score < User4Score)
                {
                    i++;
                }
                return i;
            }
        }
        public string User4Id { get; set; }
        public int? User4Score { get; set; }
        public int User4Placement
        {
            get
            {
                var i = 1;
                if (User4Score.HasValue && User2Score.HasValue && User4Score < User2Score)
                {
                    i++;
                }
                if (User4Score.HasValue && User3Score.HasValue && User4Score < User3Score)
                {
                    i++;
                }
                if (User4Score.HasValue && User1Score.HasValue && User4Score < User1Score)
                {
                    i++;
                }
                return i;
            }
        }
        public DateTime Timestamp { get; set; }
        public string FullLog { get; set; }

        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual User User3 { get; set; }
        public virtual User User4 { get; set; }

    }
}
