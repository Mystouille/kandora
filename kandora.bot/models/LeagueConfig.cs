using System;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.models
{
    public partial class LeagueConfig
    {
        public LeagueConfig(int id, bool countPoints, string eloSystem)
        {
            Id = id;
            CountPoints = countPoints;
            EloSystem = eloSystem;
        }

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

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int Id { get; set; }
        public bool CountPoints { get; set; }
        public double StartingPoints { get; set; }
        public bool AllowSanma { get; set; }
        public double Uma3p1 { get; set; }
        public double Uma3p2 { get; set; }
        public double Uma3p3 { get; set; }
        public double Uma4p1 { get; set; }
        public double Uma4p2 { get; set; }
        public double Uma4p3 { get; set; }
        public double Uma4p4 { get; set; }
        public double Oka { get; set; }
        public double PenaltyLast { get; set; }
        public double PenaltyChombo { get; set; }
        public string EloSystem { get; set; }
        public double InitialElo { get; set; }
        public double MinElo { get; set; }
        public double BaseEloChangeDampening { get; set; }
        public double EloChangeStartRatio { get; set; }
        public double EloChangeEndRatio { get; set; }
        public int TrialPeriodDuration { get; set; }

        public double getPostGameBonus(string playerPlacement, int nbPlayers, int nbChombos)
        {
            int nbOpponents = nbPlayers-1;
            double[] umas = new double[4];
            if (nbPlayers == 3)
            {
                umas[0] = Uma3p1 * 1000;
                umas[1] = Uma3p2 * 1000;
                umas[2] = Uma3p3 * 1000;
                umas[3] = 0;
            }
            else
            {
                umas[0] = Uma4p1 * 1000;
                umas[1] = Uma4p2 * 1000;
                umas[2] = Uma4p3 * 1000;
                umas[3] = Uma4p4 * 1000;
            }
            double bonusPts = 0;
            if (playerPlacement.Contains("1"))
            {
                bonusPts += (umas[0] + Oka * 1000 * nbOpponents) / playerPlacement.Length;
            }
            if (playerPlacement.Contains("2"))
            {
                bonusPts += (umas[1] - Oka * 1000) / playerPlacement.Length;
            }
            if (playerPlacement.Contains("3"))
            {
                bonusPts += (umas[2] - Oka * 1000) / playerPlacement.Length;
                if (nbOpponents == 3)
                {
                    bonusPts -= PenaltyLast * 1000 / playerPlacement.Length;
                }
            }
            if (nbPlayers == 4 && playerPlacement.Contains("4"))
            {
                bonusPts += (umas[3] - Oka * 1000 - PenaltyLast * 1000) / playerPlacement.Length;
            }
            return bonusPts - nbChombos * PenaltyChombo * 1000;
        }

        public double getNewRanking( List<UserGameData> dataList, string userId)
        {
            var userData = dataList.Where(x => x.UserId == userId).FirstOrDefault();
            var ownRankingHistory = userData.RankingHistory;
            var ownPosition = userData.UserPlacement;
            var otherPlayerLastRankings = dataList.Where(data => data.UserId != userId).Select(x => x.RankingHistory.Last());
            var ownScore = userData.UserScore;
            var ownChombos = userData.UserChombo;
            var oldRank = EloSystem == "Full" ? InitialElo : 0;
            if (userData.RankingHistory.Count > 0)
            {
                oldRank = userData.RankingHistory.Last().NewRank;
            }

            int nbOpponents = otherPlayerLastRankings.Count();
            int nbTotalGames = ownRankingHistory.Where(x => x.OldRank != null).Count();
            double avgOpponentRk = (otherPlayerLastRankings.Sum(x => x.NewRank)) / nbOpponents;
            double[] UMA = nbOpponents == 3
                ? new double[] { Uma4p1, Uma4p2, Uma4p3, Uma4p4 }
                : new double[] { Uma3p1, Uma3p2, Uma3p3 };

            //Rank affected by score (UMA count also for ELO system, since they are the base ELO variation)

            double basePts = (CountPoints ? (ownScore - StartingPoints * 1000) : 0).GetValueOrDefault(); //SCORE

            basePts += getPostGameBonus(ownPosition, dataList.Count, ownChombos); // Oka, Uma, last place penalty

            double rankChange = basePts;
            var newRank = oldRank;
            //ELO bonus/penalty depending on opponents average ELO
            if (EloSystem == "Full")
            {
                double baseEloChange = avgOpponentRk - oldRank;
                double dampenedEloChange = (baseEloChange / BaseEloChangeDampening); //Moderating the bonus
                double finalEloChange = Math.Max(dampenedEloChange, -1 * (UMA[0] + 3 * Oka)); //First player cannot lose more than his UMA+OK
                finalEloChange = Math.Min(finalEloChange, -1 * (UMA[nbOpponents] - PenaltyLast)); //Last player cannot win more than his UMA+PENALTY
                rankChange += finalEloChange;
                //Moderating the whole rank change
                double currentDeltaRatio = EloChangeStartRatio + (EloChangeEndRatio - EloChangeStartRatio) * (Convert.ToDouble(Math.Min(nbTotalGames, TrialPeriodDuration)) / Convert.ToDouble(TrialPeriodDuration));
                double finalRankChange = rankChange * currentDeltaRatio;

                newRank = oldRank + finalRankChange;
            }
            else if (EloSystem == "Simple")
            {
                double expectedGain = oldRank - avgOpponentRk;
                rankChange *= 1000;
                rankChange -= expectedGain;
                newRank = oldRank + rankChange / BaseEloChangeDampening;
            }
            else if (EloSystem == "Average")
            {
                newRank = ((double)nbTotalGames * oldRank + basePts) / (double)(nbTotalGames + 1);
            }
            else if (EloSystem == "None")
            {
                newRank += basePts;
            }
            if (EloSystem == "Full" && MinElo != -1)
            {
                newRank = Math.Max(MinElo, newRank);
            }
            return newRank;
        }

    }
}
