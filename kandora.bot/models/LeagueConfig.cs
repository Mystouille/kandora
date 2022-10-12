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
        public string EloSystem { get; set; }
        public double InitialElo { get; set; }
        public double MinElo { get; set; }
        public double BaseEloChangeDampening { get; set; }
        public double EloChangeStartRatio { get; set; }
        public double EloChangeEndRatio { get; set; }
        public int TrialPeriodDuration { get; set; }

    }
}
