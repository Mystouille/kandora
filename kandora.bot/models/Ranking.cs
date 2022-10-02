using System;
using System.Collections.Generic;
using System.Linq;

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
        public int? Position { get; set; }
        public DateTime Timestamp { get; set; }
        public string GameId { get; set; }
        public string ServerId { get; set; }

        public Ranking(int id, string userId, float oldRank, float newRank, int position, DateTime timestamp, string gameId, string serverId)
        {
            Id = id;
            UserId = userId;
            OldRank = oldRank;
            NewRank = newRank;
            Position = position;
            Timestamp = timestamp;
            GameId = gameId;
            ServerId = serverId;
        }

        public Ranking(string userId, List<Ranking> userOldRks, Ranking oldRk2, Ranking oldRk3, Ranking oldRk4, int position, string gameId, string serverId, LeagueConfig config)
        {
            UserId = userId;
            Position = position;
            GameId = gameId;
            ServerId = serverId;
            if(userOldRks.Count > 0)
            {
                OldRank = userOldRks.Last().NewRank;
            }
            else
            {
                OldRank = config.InitialElo;
            }
            NewRank = getNewElo(userOldRks, position, new Ranking[] { oldRk2, oldRk3, oldRk4 }, config);
        }

        private double getNewElo(List<Ranking> ownRankingHistory, int ownPosition, Ranking[] otherPlayerRankings, LeagueConfig cf, double ownScore = (double)0)
        {

            double oldRank = OldRank.HasValue ? cf.InitialElo : OldRank.Value;
            int nbOpponents = otherPlayerRankings.Length;
            int nbTotalGames = ownRankingHistory.Count;
            double avgOpponentRk = (otherPlayerRankings.Sum(x=>x.NewRank)) / nbOpponents;
            double[] UMA = nbOpponents == 3
                ? new double[] { cf.Uma4p1, cf.Uma4p2, cf.Uma4p3, cf.Uma4p4 }
                : new double[] { cf.Uma3p1, cf.Uma3p2, cf.Uma3p3 };

            //Rank affected by score (UMA count also for ELO system, since they are the base ELO variation)
            double basePts =
                UMA[ownPosition - 1] //UMA
                + (ownPosition == 1 ? cf.Oka * nbOpponents : -cf.Oka) //OKA
                - (ownPosition == nbOpponents+1 ? cf.PenaltyLast : 0); //PENALTY
            basePts += cf.CountPoints ? (ownScore - cf.StartingPoints) : 0; //SCORE

            double rankChange = basePts;
            //ELO bonus/penalty depending on opponents average ELO
            if (cf.UseEloSystem)
            {
                double baseEloChange = avgOpponentRk - oldRank;
                double dampenedEloChange = (baseEloChange / cf.BaseEloChangeDampening); //Moderating the bonus
                double finalEloChange = Math.Max(dampenedEloChange, -1 * (UMA[0] + 3 * cf.Oka)); //First player cannot lose more than his UMA+OK
                finalEloChange = Math.Min(finalEloChange, -1 * (UMA[nbOpponents] - cf.PenaltyLast)); //Last player cannot win more than his UMA+PENALTY
                rankChange += finalEloChange;
            }

            //Moderating the whole rank change
            double currentDeltaRatio = cf.EloChangeStartRatio + (cf.EloChangeEndRatio - cf.EloChangeStartRatio) * (Convert.ToDouble(Math.Min(nbTotalGames, cf.TrialPeriodDuration)) / Convert.ToDouble(cf.TrialPeriodDuration));
            double finalRankChange = rankChange * currentDeltaRatio;

            var newRank = oldRank + finalRankChange;

            if (cf.UseEloSystem)
            {
                newRank = Math.Max(cf.MinElo, newRank);
            }
            return newRank;
        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
