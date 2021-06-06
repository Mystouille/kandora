using System;
using System.Collections.Generic;
using System.Text;

namespace kandora.bot.http
{
    public class TenhouGame
    {
        public string Ver { get; set; }
        public string Ref { get; set; }
        public List<Round> Log { get; set; }
        public string Ratingc { get; set; }
        public Rule Rule { get; set; }
        public int Lobby { get; set; }
        public string[] Dan { get; set; }
        public int[] Rate { get; set; }
        public string[] Sx { get; set; }
        public int[] FinalScores { get; set; }
        public float[] FinalRankDeltas { get; set; }
        public string[] Name { get; set; }
        public string[] Title { get; set; }
        public string Pretty()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Title: {this.Title[0]}\n");
            sb.Append($"Time: {this.Title[1]}\n");
            sb.Append($"Scores: \n");
            var maxLen = 0;
            for (int i = 0; i < Name.Length; i++)
            {
                maxLen = Name[i].Length > maxLen ? Name[i].Length : maxLen;
            }
            for (int i = 0; i<Name.Length; i++)
            {
                var name = Name[i].PadRight(maxLen);
                sb.Append($"{name}:\t{FinalScores[i]}\t({FinalRankDeltas[i]})\n");
            }
            var bestPayment = 0;
            RoundResult bestResult = null;
            Round bestRound = null;
            var player = -1;
            foreach (var log in Log)
            {
                foreach(var res in log.Result)
                {
                    var i = 0;
                    foreach(var delta in res.Payments)
                    {
                        if( delta > bestPayment)
                        {
                            bestPayment = delta;
                            bestResult = res;
                            bestRound = log;
                            player = i;
                        }
                        i++;
                    }
                }
            }
            sb.Append($"Best hand: {this.Name[player]} (round {bestRound.RoundNumber}) with {bestResult.HandScore} for {bestPayment} total \n");
            return sb.ToString();
        }

        public int WinnerInt
        {
            get
            {
                var winner = 0;
                for (int i = 1; i < FinalScores.Length; i++)
                {
                    winner = FinalScores[i] > FinalScores[winner] ? i : winner;
                }
                return winner;
            }
        }
    }

    public class TenhouLog
    {
        public Round[] Rounds { get; set; }
    }

    public class Round
    {
        public int RoundNumber { get; set; }
        public int NbHonbas { get; set; }
        public int NbRiichi { get; set; }
        public int[] StartingScores { get; set; }
        public int[] Doras { get; set; }
        public int[] UraDoras { get; set; }
        public string[][] HaiPais { get; set; }
        public string[][] Draws { get; set; }
        public string[][] Discards { get; set; }
        public RoundResult[] Result { get; set; }

    }

    public class RoundResult
    {
        public string Name { get; set; }
        public int[] Payments { get; set; }
        public int Winner { get; set; }
        public int Loser { get; set; } //Tsumo: Loser=Winner
        public int LoserPao { get; set; } //No pao: LoserPao=Winner
        public string HandScore { get; set; }
        public string[] Yakus { get; set; }
    }

    public class Rule
    {
        public string Disp { get; set; }
        public int Aka53 { get; set; }
        public int Aka52 { get; set; }
        public int Aka51 { get; set; }
    }
}
