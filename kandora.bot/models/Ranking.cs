using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace kandora.bot.models
{
    public partial class Ranking
    {
        /**
        // === Values related to Riichi rules ===

        // Payers start with thes points
        public static readonly float STARTING_PTS = 30;
        // True = the points gained/lost should be added to the OKA+UMA
        // False = Only the OKA+UMA are counted in the score
        public static readonly bool COUNT_POINTS = false;
        // Bonus points depending on the game final ranking
        public static readonly float[] UMA4 = new float[] { 30, 10, -10, -30 };
        public static readonly float[] UMA3 = new float[] { 20, 0, -20 };
        // Bonus point paid to the game winner by the other players
        public static readonly float OKA = (float)0;
        // Penalty of the last place
        public static readonly float PENALTY_LAST = (float)0;


        // === Values related to ELO system ===

        public static bool USE_ELO_SYSTEM = true;
        //Players start here
        public static readonly float INITIAL_ELO = 1500;
        //Can't go lower
        public static readonly float MIN_ELO = 1200;
        //Moderate the game result impact on the ELO
        public static readonly float BASE_ELO_CHANGE_DAMPENING = 10;
        //Divide the final ELO change by this value
        public static readonly float ELO_CHANGE_START_RATIO = 1;
        public static readonly float ELO_CHANGE_END_RATIO = (float)0.2;
        //Number of games after which the player will always have the same ELO change ratio
        public static readonly int  TRIAL_PERIOD_DURATION = 80;

        */
        public int Id { get; set; }
        public string UserId { get; set; }
        public double? OldRank { get; set; }
        public double NewRank { get; set; }
        public string Position { get; set; }
        public int? Score { get; set; }
        public DateTime Timestamp { get; set; }
        public string GameId { get; set; }
        public string ServerId { get; set; }

        public Ranking(int id, string userId, float oldRank, float newRank, string position, DateTime timestamp, string gameId, string serverId, int score = 0)
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
        }

        public Ranking(string userId, List<Ranking> userOldRks, Ranking oldRk2, Ranking oldRk3, Ranking oldRk4, string position, string gameId, string serverId, LeagueConfig config, int score = 0)
        {
            UserId = userId;
            Position = position;
            GameId = gameId;
            Score = score;
            ServerId = serverId;
            if (userOldRks.Count > 0)
            {
                OldRank = userOldRks.Last().NewRank;
            }
            else
            {
                OldRank = config.EloSystem == "Full" ? config.InitialElo : 0;
            }
            NewRank = getNewElo(userOldRks, position, new Ranking[] { oldRk2, oldRk3, oldRk4 }, config, score);
        }

        private double getNewElo(List<Ranking> ownRankingHistory, string ownPosition, Ranking[] otherPlayerRankings, LeagueConfig cf, int ownScore = 0)
        {

            double oldRank = OldRank.HasValue ? OldRank.Value : cf.InitialElo;
            int nbOpponents = otherPlayerRankings.Length;
            int nbTotalGames = ownRankingHistory.Count;
            double avgOpponentRk = (otherPlayerRankings.Sum(x=>x.NewRank)) / nbOpponents;
            double[] UMA = nbOpponents == 3
                ? new double[] { cf.Uma4p1, cf.Uma4p2, cf.Uma4p3, cf.Uma4p4 }
                : new double[] { cf.Uma3p1, cf.Uma3p2, cf.Uma3p3 };

            //Rank affected by score (UMA count also for ELO system, since they are the base ELO variation)

            double basePts = 0;
            if (ownPosition.Contains("1"))
            {
                basePts += (UMA[0] + cf.Oka*nbOpponents)/ownPosition.Length;
            }
            if (ownPosition.Contains("2"))
            {
                basePts += (UMA[1] - cf.Oka) / ownPosition.Length;
            }
            if (ownPosition.Contains("3"))
            {
                basePts += (UMA[2] - cf.Oka) / ownPosition.Length;
                if(nbOpponents == 3)
                {
                    basePts -= cf.PenaltyLast / ownPosition.Length;
                }
            }
            if (ownPosition.Contains("4"))
            {
                basePts += (UMA[3] - cf.Oka- cf.PenaltyLast) / ownPosition.Length;
            }
            basePts += cf.CountPoints ? (ownScore - cf.StartingPoints)/1000 : 0; //SCORE

            double rankChange = basePts;
            var newRank = oldRank;
            //ELO bonus/penalty depending on opponents average ELO
            if (cf.EloSystem == "Full")
            {
                double baseEloChange = avgOpponentRk - oldRank;
                double dampenedEloChange = (baseEloChange / cf.BaseEloChangeDampening); //Moderating the bonus
                double finalEloChange = Math.Max(dampenedEloChange, -1 * (UMA[0] + 3 * cf.Oka)); //First player cannot lose more than his UMA+OK
                finalEloChange = Math.Min(finalEloChange, -1 * (UMA[nbOpponents] - cf.PenaltyLast)); //Last player cannot win more than his UMA+PENALTY
                rankChange += finalEloChange;
                //Moderating the whole rank change
                double currentDeltaRatio = cf.EloChangeStartRatio + (cf.EloChangeEndRatio - cf.EloChangeStartRatio) * (Convert.ToDouble(Math.Min(nbTotalGames, cf.TrialPeriodDuration)) / Convert.ToDouble(cf.TrialPeriodDuration));
                double finalRankChange = rankChange * currentDeltaRatio;

                newRank = oldRank + finalRankChange;
            } else if(cf.EloSystem == "Simple")
            {
                double expectedGain = oldRank - avgOpponentRk;
                rankChange *= 1000;
                rankChange -= expectedGain;
                newRank = oldRank + rankChange/cf.BaseEloChangeDampening;
            }
            if (cf.EloSystem == "Full" && cf.MinElo != -1)
            {
                newRank = Math.Max(cf.MinElo, newRank);
            }
            return newRank;
        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
