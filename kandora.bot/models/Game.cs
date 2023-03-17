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

        public Game(int id, string gameName, Server server, string user1Id, string user2Id, string user3Id, string user4Id, GameType platform, DateTime timestamp, bool isSanma ) : this(server)
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
        }
        public int Id { get; set; }
        public string GameName { get; set; }
        public Server Server { get; set; }
        public GameType Platform { get; set; }
        public bool IsSanma { get; set; }   
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
        private string getMissingPreplaces()
        {
            var places = User4Score == null ? "123" : "1234";
            places.Replace(User1Score.ToString(), "");
            places.Replace(User2Score.ToString(), "");
            places.Replace(User3Score.ToString(), "");
            if (!IsSanma)
            {
                places.Replace(User4Score.ToString(), "");
            }
            return places;
        }

        public string User1Id { get; set; }
        public int? User1Score { get; set; }
        private int User1PrePlace { get {
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
        public string User1Placement
        {
            get
            {
                var place = "";
                int i = 0;
                if (User1PrePlace == User2PrePlace)
                {
                    i++;
                }
                if (User1PrePlace == User3PrePlace)
                {
                    i++;
                }
                if (User1PrePlace == User4PrePlace)
                {
                    i++;
                }
                for(int a = User1PrePlace; a <= User1PrePlace+i; a++)
                {
                    place += a.ToString();
                }
                return place;
            }
        }
        public string User2Id { get; set; }
        public int? User2Score { get; set; }
        private int User2PrePlace
        {
            get
            {
                var i = 1;
                if (User2Score.HasValue && User1Score.HasValue && User2Score < User1Score)
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
        public string User2Placement
        {
            get
            {
                var place = "";
                int i = 0;
                if (User2PrePlace == User1PrePlace)
                {
                    i++;
                }
                if (User2PrePlace == User3PrePlace)
                {
                    i++;
                }
                if (User2PrePlace == User4PrePlace)
                {
                    i++;
                }
                for (int a = User2PrePlace; a <= User2PrePlace + i; a++)
                {
                    place += a.ToString();
                }
                return place;
            }
        }
        public string User3Id { get; set; }
        public int? User3Score { get; set; }
        private int User3PrePlace
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
        public string User3Placement
        {
            get
            {
                var place = "";
                int i = 0;
                if (User3PrePlace == User1PrePlace)
                {
                    i++;
                }
                if (User3PrePlace == User2PrePlace)
                {
                    i++;
                }
                if (User3PrePlace == User4PrePlace)
                {
                    i++;
                }
                for (int a = User3PrePlace; a <= User3PrePlace + i; a++)
                {
                    place += a.ToString();
                }
                return place;
            }
        }
        public string User4Id { get; set; }
        public int? User4Score { get; set; }
        private int User4PrePlace
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
        public string User4Placement
        {
            get
            {
                var place = "";
                int i = 0;
                if (User4PrePlace == User1PrePlace)
                {
                    i++;
                }
                if (User4PrePlace == User3PrePlace)
                {
                    i++;
                }
                if (User4PrePlace == User2PrePlace)
                {
                    i++;
                }
                for (int a = User4PrePlace; a <= User4PrePlace + i; a++)
                {
                    place += a.ToString();
                }
                return place;
            }
        }
        public DateTime Timestamp { get; set; }
        public string FullLog { get; set; }

        public int User1Chombo { get; set; }
        public int User2Chombo { get; set; }
        public int User3Chombo { get; set; }
        public int User4Chombo { get; set; }
    }
}
