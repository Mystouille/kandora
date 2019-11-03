using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kandora
{
    class Ranking
    {
        public Ranking(int id, ulong userId, double oldElo, double newElo, int position, DateTime timestamp, int gameId)
        {
            Id = id;
            UserId = userId;
            OldElo = oldElo;
            NewElo = newElo;
            Position = position;
            TimeStamp = timestamp;
            GameId = gameId;
        }

        public Ranking(ulong userId, List<Ranking> userOldRks, Ranking oldRk2, Ranking oldRk3, Ranking oldRk4, int position, int gameId)
        {
            UserId = userId;
            Position = position;
            GameId = gameId;
            OldElo = userOldRks.LastOrDefault().NewElo;

            double avgTableRk = (oldRk2.NewElo + oldRk3.NewElo + oldRk4.NewElo) / 3;
            int basePts = BASE_PTS[position - 1];

            //Decreases from 1 to 0.2 for the first 80 games
            double adjustment = Math.Min(1 - (userOldRks.Count * 0.01), 0.2);

            double delta = adjustment * (basePts + (avgTableRk - OldElo) / DAMPENING);

            NewElo = OldElo + delta;
        }

        public static readonly double INITIAL_ELO = 1500;
        public static readonly double MIN_ELO = 1200;
        public static readonly int DAMPENING = 15;
        public static readonly int[] BASE_PTS = new int[] { 30, 10, -10, -30 };

        public int Id { get; }
        public ulong UserId { get; }
        public double OldElo { get; }
        public double NewElo { get; }
        public int Position { get; }
        public DateTime TimeStamp { get; }
        public int GameId { get; }

    }
}
