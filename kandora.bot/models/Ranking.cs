using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace kandora.bot.models
{
    public partial class Ranking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double? OldRank { get; set; }
        public double NewRank { get; set; }
        public string Position { get; set; }
        public int? Score { get; set; }
        public DateTime Timestamp { get; set; }
        public int GameId { get; set; }
        public string ServerId { get; set; }

        public double ScoreWithBonus { get; set; }

        public Ranking(int id, string userId, float? oldRank, float newRank, string position, double finalScore, DateTime timestamp, int gameId, string serverId, int score = 0)
        {
            Id = id;
            UserId = userId;
            OldRank = oldRank;
            NewRank = newRank;
            Position = position;
            Timestamp = timestamp;
            GameId = gameId;
            ServerId = serverId;
            Score = score;
            ScoreWithBonus = finalScore;
        }

        public Ranking(string userId, List<UserGameData> dataList, int gameId, string serverId, LeagueConfig config)
        {
            var userData = dataList.Where(x => x.UserId == userId).FirstOrDefault();
            UserId = userId;
            Position = userData.UserPlacement;
            GameId = gameId;
            Score = userData.UserScore;
            ServerId = serverId;
            if (userData.RankingHistory.Count > 0)
            {
                OldRank = userData.RankingHistory.Last().NewRank;
            }
            else
            {
                OldRank = config.EloSystem == "Full" ? config.InitialElo : 0;
            }
            NewRank = config.getNewRanking(dataList, userId);

            ScoreWithBonus = Score.GetValueOrDefault() + config.getPostGameBonus(Position, dataList.Count, userData.UserChombo);
        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
