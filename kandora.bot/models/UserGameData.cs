using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.models
{
    public class UserGameData
    {
        public UserGameData(string userId, int? userScore, int userChombo, List<Ranking> rankingHistory)
        {
            UserId = userId;
            UserScore = userScore;
            UserChombo = userChombo;
            UserPlacement = "";
            RankingHistory = rankingHistory;
        }

        public string UserId { get; set; }
        public int? UserScore { get; set; }
        public int UserChombo { get; set; }
        public string UserPlacement { get; set; }
        public List<Ranking> RankingHistory { get; set; }

    }
}
