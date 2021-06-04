using System;
using System.Collections.Generic;

namespace kandora.bot.models
{
    public partial class Game
    {
        private ICollection<Ranking> ranking;

        public Game(Server server)
        {
            this.Server = server;

            ranking = new HashSet<Ranking>();
        }

        public Game(int id, Server server, string user1Id, string user2Id, string user3Id, string user4Id, bool user1Signed, bool user2Signed, bool user3Signed, bool user4Signed) : this(server)
        {
            this.Id = id;
            this.User1Id = user1Id;
            this.User2Id = user2Id;
            this.User3Id = user3Id;
            this.User4Id = user4Id;
            this.User1Signed = user1Signed;
            this.User2Signed = user2Signed;
            this.User3Signed = user3Signed;
            this.User4Signed = user4Signed;
        }
        public int Id { get; set; }
        public Server Server { get; set; }
        public string GameId { get; set; }
        public string Platform { get; set; }
        public string User1Id { get; set; }
        public int? User1Score { get; set; }
        public bool User1Signed { get; set; }
        public string User2Id { get; set; }
        public int? User2Score { get; set; }
        public bool User2Signed { get; set; }
        public string User3Id { get; set; }
        public int? User3Score { get; set; }
        public bool User3Signed { get; set; }
        public string User4Id { get; set; }
        public int? User4Score { get; set; }
        public bool User4Signed { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual User User3 { get; set; }
        public virtual User User4 { get; set; }
        public virtual ICollection<Ranking> Ranking { get => ranking;}
        public bool IsSignedOff
        {
            get
            {
                return User1Signed && User2Signed && User3Signed && User4Signed;
            }
        }

        public bool isSignedBy (string userId)
        {
            return userId == User1Id && User1Signed
                || userId == User2Id && User2Signed
                || userId == User3Id && User3Signed
                || userId == User4Id && User4Signed;
        }

        public void signBy(string userId)
        {
            if(userId == User1Id)
            {
                User1Signed = true;
            } else if (userId == User2Id)
            {
                User2Signed = true;
            }
            else if (userId == User3Id)
            {
                User3Signed = true;
            }
            else if (userId == User4Id)
            {
                User4Signed = true;
            }
        }
    }
}
