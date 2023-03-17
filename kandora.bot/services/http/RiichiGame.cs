using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.models;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace kandora.bot.http
{
    public class RiichiGame
    {
        public GameType GameType { get; set; }
        
        public string GameTypeStr
        {
            get
            {
                switch (this.GameType)
                {
                    case GameType.Mahjsoul:
                        return "Mahjsoul";
                    case GameType.Tenhou:
                        return "Tenhou";
                    case GameType.IRL:
                        return "IRL";
                    default: return "IRL";
                }
            }
        }
        public string Ref { get; set; }
        public List<Round> Rounds { get; set; }
        public string Ratingc { get; set; }
        public Rule Rule { get; set; }
        public int Lobby { get; set; }
        public string[] Dan { get; set; }
        public float[] Rate { get; set; }
        public string[] Sx { get; set; }
        public int[] FinalScores { get; set; }
        public float[] FinalRankDeltas { get; set; }
        public string[] Names { get; set; }
        public string[] UserIds { get; set; }
        public string[] Title { get; set; }
        public string FullLog { get; set; }
        public DateTime Timestamp { get; set; }
        public int MahjsoulRoomId
        {
            get
            {
                try
                {
                    var split = this.Title[0].Split(':');
                    return Int32.Parse(split[split.Length - 1]);
                }
                catch
                {
                    return -1;
                }
            }
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
