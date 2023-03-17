using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace kandora.bot.utils.TensoulLogParser;

public static class TensoulParser
{
    public static RiichiGame ParseTensoulFormatGame(string payload, GameType gameType)
    {
        var game = JsonSerializer.Deserialize<TensoulGame>(payload);
        var node = JsonNode.Parse(payload);
        var riichiGame = game.ToRiichiGame(node.SelectByPath("$.log"));
        riichiGame.GameType = gameType;

        return riichiGame;
    }

    public static RiichiGame ToRiichiGame(this TensoulGame tenhouGame, JsonNode resultGame)
    {
        return new RiichiGame()
        {
            Dan = tenhouGame.Dans,
            Ref = tenhouGame.Reference,
            Rule = tenhouGame.Rule.ToRiichiRule(),
            FinalRankDeltas = tenhouGame.Scores.Where((s, i) => i % 2 == 1).ToArray(),
            FinalScores = tenhouGame.Scores.Where((s, i) => i % 2 == 0).Select(s => (int)s).ToArray(),
            Names = tenhouGame.Names,
            UserIds = tenhouGame.GetIds(),
            Rate = tenhouGame.Rate,
            Lobby = tenhouGame.Lobby,
            Sx = tenhouGame.Sx,
            Ratingc = tenhouGame.Rating,
            Title = tenhouGame.Title,
            Rounds = GetRiichiRounds(resultGame.AsArray()),
            Timestamp = GetTimestamp(tenhouGame.Reference, tenhouGame.Title)
        };
    }

    static DateTime GetTimestamp(string Ref, string[] Title)
    {
        var dateStr = Ref.Substring(0, 10);
        //Tenhou time is if the ref value
        if (int.TryParse(dateStr, out _))
        {
            return DateTime.ParseExact(dateStr, "yyyyMMddHH", System.Globalization.CultureInfo.InvariantCulture);
        }
        //majsoul as well, but there isn't the hour info, and the title value is more accurate, so let's use that
        else if (Title.Length > 1)
        {
            return DateTime.ParseExact(Title[1], "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            throw new Exception(Resources.commandError_CouldntExtractDateFromLog);
        }
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
                if (rtr.Count <= 2)
                {
                    var roundResult = new RoundResult()
                    {
                        Name = rtr[0].GetValue<string>(),
                        Payments = rtr[1].AsArray().ToArray<int>()
                    };
                    roundResults.Add(roundResult);
                }
                else
                {
                    for(int k = 1; k < rtr.Count; k += 2)
                    {
                        var roundResult = new RoundResult()
                        {
                            Name = rtr[0].GetValue<string>(),
                            Payments = rtr[k].AsArray().ToArray<int>(),
                            Winner = rtr[k + 1].AsArray()[0].GetValue<int>(),
                            Loser = rtr[k + 1].AsArray()[1].GetValue<int>(),
                            LoserPao = rtr[k + 1].AsArray()[2].GetValue<int>(),
                            HandScore = rtr[k + 1].AsArray()[3].GetValue<string>(),
                            Yakus = rtr[k + 1].AsArray().ToStringArray().Skip(4).ToArray(),
                        };
                        roundResults.Add(roundResult);
                    }
                }
            }
            round.Result = roundResults.ToArray();
            rounds.Add(round);
        }

        return rounds;
    }
}
