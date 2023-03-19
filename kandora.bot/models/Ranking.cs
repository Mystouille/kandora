using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace kandora.bot.models
{
    public partial class Ranking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double? OldRank { get; set; }
        public double NewRank { get; set; }
        public string Position { get; set; }
        public int? Score { get; set; }
        public DateTime Timestamp { get; set; }
        public int GameId { get; set; }
        public string ServerId { get; set; }

        public double ScoreWithBonus { get; set; }

        public Ranking(int id, string userId, float? oldRank, float newRank, string position, double finalScore, DateTime timestamp, int gameId, string serverId, int score = 0)
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
            ScoreWithBonus = finalScore;
        }

        public Ranking(string userId, List<UserGameData> dataList, int gameId, string serverId, LeagueConfig config)
        {
            var userData = dataList.Where(x => x.UserId == userId).FirstOrDefault();
            UserId = userId;
            Position = userData.UserPlacement;
            GameId = gameId;
            Score = userData.UserScore;
            ServerId = serverId;
            if (userData.RankingHistory.Count > 0)
            {
                OldRank = userData.RankingHistory.Last().NewRank;
            }
            else
            {
                OldRank = config.EloSystem == "Full" ? config.InitialElo : 0;
            }
            NewRank = config.getNewRanking(dataList, userId);

            ScoreWithBonus = config.getFinalPoints(Position, Score, dataList.Count, userData.UserChombo);
        }

        public static Ranking[] getPartialRanking(Game game, LeagueConfig config)
        {

            var dataList = getPartialUserGameInfos(game);

            var newRkList = dataList.Select(userData => new Ranking(userData.UserId, dataList, game.Id, game.Server.Id, config)).ToArray();
            return newRkList;
        }

        // get user game data without the ranking history (needed to compute change in ranking)
        // usefull to get the placement and the scores after bonuses
        private static List<UserGameData> getPartialUserGameInfos(Game game)
        {

            var dataList = new List<UserGameData>();

            dataList.Add(new UserGameData(game.User1Id, game.User1Score, game.User1Chombo, new List<Ranking>()));
            dataList.Add(new UserGameData(game.User2Id, game.User2Score, game.User2Chombo, new List<Ranking>()));
            dataList.Add(new UserGameData(game.User3Id, game.User3Score, game.User3Chombo, new List<Ranking>()));
            if (!game.IsSanma)
            {
                dataList.Add(new UserGameData(game.User4Id, game.User4Score, game.User4Chombo, new List<Ranking>()));
            }

            dataList = dataList.OrderBy(x => x.UserScore).ToList();
            dataList.Reverse();

            Ranking.addPlacementToDataList(dataList,game);

            return dataList;
        }

        public static void addPlacementToDataList(List<UserGameData> dataList, Game game)
        {

            // Computing placement (two player exaequo for 2nd and 3rd place have a placement value of "23")

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].UserPlacement = $"{i + 1}";
            }
            for (int i = 0; i < dataList.Count - 1; i++)
            {
                if (dataList[i].UserScore == dataList[i + 1].UserScore)
                {
                    dataList[i].UserPlacement = $"{dataList[i].UserPlacement}{i + 2}";
                    dataList[i + 1].UserPlacement = $"{i + i}{dataList[i + 1].UserPlacement}";
                }
            }
            for (int i = 0; i < dataList.Count - 1; i++)
            {
                if (dataList[i].UserScore == dataList[i + 1].UserScore)
                {
                    dataList[i].UserPlacement = $"{i + 1}{i + 2}";
                    dataList[i + 1].UserPlacement = $"{i + 1}{i + 2}";
                }
            }
            for (int i = 0; i < dataList.Count - 2; i++)
            {
                if (dataList[i].UserScore == dataList[i + 1].UserScore && dataList[i].UserScore == dataList[i + 2].UserScore)
                {
                    dataList[i].UserPlacement = $"{i + 1}{i + 2}{i + 3}";
                    dataList[i + 1].UserPlacement = $"{i + 1}{i + 2}{i + 3}";
                    dataList[i + 2].UserPlacement = $"{i + 1}{i + 2}{i + 3}";
                }
            }
            if (!game.IsSanma)
            {
                for (int i = 0; i < dataList.Count - 3; i++)
                {
                    if (dataList[i].UserScore == dataList[i + 1].UserScore && dataList[i].UserScore == dataList[i + 2].UserScore && dataList[i].UserScore == dataList[i + 3].UserScore)
                    {
                        dataList[i].UserPlacement = $"{i + 1}{i + 2}{i + 3}{i + 4}";
                        dataList[i + 1].UserPlacement = $"{i + 1}{i + 2}{i + 3}{i + 4}";
                        dataList[i + 2].UserPlacement = $"{i + 1}{i + 2}{i + 3}{i + 4}";
                        dataList[i + 3].UserPlacement = $"{i + 1}{i + 2}{i + 3}{i + 4}";
                    }
                }
            }

        }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
