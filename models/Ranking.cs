using System;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.models
{
    public partial class Ranking
    {
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


        public int Id { get; set; }
        public string UserId { get; set; }
        public float? OldElo { get; set; }
        public float NewElo { get; set; }
        public int? Position { get; set; }
        public DateTime Timestamp { get; set; }
        public string GameId { get; set; }
        public string ServerId { get; set; }

        public Ranking(int id, string userId, float oldElo, float newElo, int position, DateTime timestamp, string gameId, string serverId)
        {
            Id = id;
            UserId = userId;
            OldElo = oldElo;
            NewElo = newElo;
            Position = position;
            Timestamp = timestamp;
            GameId = gameId;
            ServerId = serverId;
        }

        public Ranking(string userId, List<Ranking> userOldRks, Ranking oldRk2, Ranking oldRk3, Ranking oldRk4, int position, string gameId, string serverId)
        {
            UserId = userId;
            Position = position;
            GameId = gameId;
            ServerId = serverId;
            OldElo = userOldRks.LastOrDefault().NewElo;
            NewElo = getNewElo(userOldRks, position, new Ranking[] { oldRk2, oldRk3, oldRk4 });
        }

        private float getNewElo(List<Ranking> ownRankingHistory, int ownPosition, Ranking[] otherPlayerRankings, float ownScore = (float)0)
        {
            float oldElo = OldElo.HasValue ? INITIAL_ELO : OldElo.Value;
            int nbOpponents = otherPlayerRankings.Length;
            int nbTotalGames = ownRankingHistory.Count;
            float avgOpponentRk = (otherPlayerRankings.Sum(x=>x.NewElo)) / nbOpponents;
            float[] UMA = nbOpponents == 3 ? UMA4 : UMA3;

            //ELO affected by score
            float basePts=
                (ownScore - STARTING_PTS) //SCORE
                + UMA[ownPosition - 1] //UMA
                + (ownPosition == 1 ? OKA * nbOpponents : -OKA) //OKA
                - (ownPosition == nbOpponents+1 ? PENALTY_LAST : 0); //PENALTY

            float rankChange = basePts;
            //ELO bonus/penalty depending on opponents average ELO
            if (USE_ELO_SYSTEM)
            {
                float baseEloChange = avgOpponentRk - oldElo;
                float dampenedEloChange = (baseEloChange / BASE_ELO_CHANGE_DAMPENING); //Moderating the bonus
                float finalEloChange = Math.Max(dampenedEloChange, -1 * (UMA[0] + 3 * OKA)); //First player cannot lose more than his UMA+OK
                finalEloChange = Math.Min(finalEloChange, -1 * (UMA[nbOpponents] - PENALTY_LAST)); //Last player cannot win more than his UMA+PENALTY
                rankChange += finalEloChange;
            }

            //Moderating the whole rank change
            float currentDeltaRatio = ELO_CHANGE_START_RATIO + (ELO_CHANGE_END_RATIO - ELO_CHANGE_START_RATIO) * (Math.Max(nbTotalGames, TRIAL_PERIOD_DURATION) / TRIAL_PERIOD_DURATION);
            float finalRankChange = rankChange * currentDeltaRatio;

            var newElo = oldElo + finalRankChange;

            if (USE_ELO_SYSTEM)
            {
                newElo = Math.Max(MIN_ELO, newElo);
            }
            return newElo;
        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
