using System;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.models
{
    public partial class LeagueConfig
    {
        public LeagueConfig(int id, bool countPoints, bool useEloSystem)
        {
            Id = id;
            CountPoints = countPoints;
            UseEloSystem = useEloSystem;
        }

        public int Id { get; set; }
        public bool CountPoints { get; set; }
        public float StartingPoints { get; set; }
        public float Uma3p1 { get; set; }
        public float Uma3p2 { get; set; }
        public float Uma3p3 { get; set; }
        public float Uma4p1 { get; set; }
        public float Uma4p2 { get; set; }
        public float Uma4p3 { get; set; }
        public float Uma4p4 { get; set; }
        public float Oka { get; set; }
        public float PenaltyLast { get; set; }
        public bool UseEloSystem { get; set; }
        public float InitialElo { get; set; }
        public float MinElo { get; set; }
        public float BaseEloChangeDampening { get; set; }
        public float EloChangeStartRatio { get; set; }
        public float EloChangeEndRatio { get; set; }
        public int TrialPeriodDuration { get; set; }

    }
}
