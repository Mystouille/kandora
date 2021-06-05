using System;
using System.Collections.Generic;
using System.Linq;

namespace kandora.bot.models
{
    public partial class Ranking
    {
        public static readonly double INITIAL_ELO = 1500;
        public static readonly double MIN_ELO = 1200;
        public static readonly int DAMPENING = 10;
        public static readonly int[] BASE_PTS = new int[] { 30, 10, -10, -30 };

        public int Id { get; set; }
        public string UserId { get; set; }
        public double? OldElo { get; set; }
        public double NewElo { get; set; }
        public int? Position { get; set; }
        public DateTime Timestamp { get; set; }
        public int? GameId { get; set; }
        public string ServerId { get; set; }

        public Ranking(int id, string userId, double oldElo, double newElo, int position, DateTime timestamp, int gameId, string serverId)
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

        public Ranking(string userId, List<Ranking> userOldRks, Ranking oldRk2, Ranking oldRk3, Ranking oldRk4, int position, int gameId, string serverId)
        {
            UserId = userId;
            Position = position;
            GameId = gameId;
            ServerId = serverId;
            OldElo = userOldRks.LastOrDefault().NewElo;
            double oldElo = OldElo.HasValue ? INITIAL_ELO : OldElo.Value;
            double avgTableRk = (oldRk2.NewElo + oldRk3.NewElo + oldRk4.NewElo) / 3;
            int basePts = BASE_PTS[position - 1];

            //Decreases from 1 to 0.2 for the first 80 games
            double adjustment = Math.Min(1 - (userOldRks.Count * 0.01), 0.2);

            double delta = adjustment * (basePts + ((avgTableRk - oldElo) / DAMPENING));

            NewElo = oldElo + delta;
        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
