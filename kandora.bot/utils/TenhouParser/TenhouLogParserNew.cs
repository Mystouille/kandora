using kandora.bot.http;
using kandora.bot.models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace kandora.bot.utils.TenhouParser;

public static class TenhouLogParserNew
{
    public static RiichiGame ParseTenhouFormatGame(string payload, GameType gameType)
    {
        var game = JsonSerializer.Deserialize<TenhouGame>(payload);
        var node = JsonNode.Parse(payload);
        var riichiGame = game.ToRiichiGame(node.SelectByPath("$.log"));
        riichiGame.GameType = gameType;

        return riichiGame;
    }

    public static RiichiGame ToRiichiGame(this TenhouGame tenhouGame, JsonNode resultGame)
    {
        return new RiichiGame()
        {
            Dan = tenhouGame.Dans,
            Ref = tenhouGame.Reference,
            Ver = tenhouGame.Version,
            Rule = tenhouGame.Rule.ToRiichiRule(),
            FinalRankDeltas = tenhouGame.Scores.GetRanks(),
            FinalScores = tenhouGame.Scores.GetScores(),
            Names = tenhouGame.Names,
            UserIds = tenhouGame.GetIds(),
            Rate = tenhouGame.Rate,
            Lobby = tenhouGame.Lobby,
            Sx = tenhouGame.Sx,
            Ratingc = tenhouGame.Rating,
            Title = tenhouGame.Title,
            Rounds = GetRiichiRounds(resultGame.AsArray())
        };
    }

    static int[] GetScores(this float[] sc)
    {
        var scores = new List<int>();
        for (int i = 0; i < sc.Length; i += 2)
        {
            scores.Add((int)sc[i]);
        }
        return scores.ToArray();
    }
    static float[] GetRanks(this float[] sc)
    {
        var scores = new List<float>();
        for (int i = 1; i < sc.Length; i += 2)
        {
            scores.Add(sc[i]);
        }
        return scores.ToArray();
    }


    static List<Round> GetRiichiRounds(JsonArray jsonArray)
    {
        var rounds = new List<Round>();
        for (int i = 0; i < jsonArray.Count; i++)
        {
            var curr = jsonArray[i].AsArray();
            var round = new Round();
            round.RoundNumber = curr[0].AsArray()[0].GetValue<int>();
            round.NbHonbas = curr[0].AsArray()[1].GetValue<int>();
            round.NbRiichi = curr[0].AsArray()[2].GetValue<int>();
            round.StartingScores = curr[1].AsArray().ToArray<int>();
            round.Doras = curr[2].AsArray().ToArray<int>();
            round.UraDoras = curr[3].AsArray().ToArray<int>();
            round.HaiPais = new string[][] {
                curr[4].AsArray().ToStringArray(),
                curr[7].AsArray().ToStringArray(),
                curr[10].AsArray().ToStringArray(),
                curr[13].AsArray().ToStringArray(),
            };
            round.Draws = new string[][] {
                curr[5].AsArray().ToStringArray(),
                curr[8].AsArray().ToStringArray(),
                curr[11].AsArray().ToStringArray(),
                curr[14].AsArray().ToStringArray(),
            };
            round.Discards = new string[][] {
                curr[6].AsArray().ToStringArray(),
                curr[9].AsArray().ToStringArray(),
                curr[12].AsArray().ToStringArray(),
                curr[15].AsArray().ToStringArray(),
            };
            var roundResults = new List<RoundResult>();
            for(int j = 16; j < curr.Count; j++)
            {
                var rtr = curr[j].AsArray();
                var roundResult = new RoundResult()
                {
                    Name = rtr[0].GetValue<string>(),
                    Payments = rtr[1].AsArray().ToArray<int>()
                };
                if(rtr.Count > 2)
                {
                    roundResult.Winner = rtr[2].AsArray()[0].GetValue<int>();
                    roundResult.Loser = rtr[2].AsArray()[1].GetValue<int>();
                    roundResult.LoserPao = rtr[2].AsArray()[2].GetValue<int>();
                    roundResult.HandScore = rtr[2].AsArray()[3].GetValue<string>();
                    roundResult.Yakus = rtr[2].AsArray().ToStringArray().Skip(4).ToArray();
                }
                roundResults.Add(roundResult);
            }
            round.Result = roundResults.ToArray();
            rounds.Add(round);
        }

        return rounds;
    }
}
