using kandora.bot.http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace kandora.bot.utils
{
    class TenhouLogParser
    {
        public static TenhouGame ParseTenhouGame(string payload)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(payload));
            reader.Read();
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new System.Exception();
            }
            var tenhouGame = new TenhouGame();
            reader.Read();

            tenhouGame.Ver = ParseStrProp("ver", reader);
            tenhouGame.Ref = ParseStrProp("ref", reader);
            tenhouGame.Log = ParseLog(reader);
            tenhouGame.Ratingc = ParseStrProp("ratingc", reader);
            tenhouGame.Rule = ParseRule(reader);
            tenhouGame.Lobby = ParseIntProp("lobby", reader);
            tenhouGame.Dan = ParseStrArray(reader, "dan");
            tenhouGame.Rate = ParseIntArray(reader, "rate");
            tenhouGame.Sx = ParseStrArray(reader, "sx");
            var sc = ParseStrArray(reader, "sc");
            tenhouGame.FinalScores = GetScores(sc);
            tenhouGame.FinalRankDeltas = GetRanks(sc);
            tenhouGame.Name = ParseStrArray(reader, "name");
            tenhouGame.Title = ParseStrArray(reader, "title");
            //TODO: implement
            return tenhouGame;
        }

        private static int[] GetScores(string[] sc)
        {
            var scores = new List<int>();
            for(int i = 0; i < sc.Length; i = i + 2)
            {
                scores.Add(Int32.Parse(sc[i]));
            }
            return scores.ToArray();
        }
        private static float[] GetRanks(string[] sc)
        {
            var scores = new List<float>();
            for (int i = 1; i < sc.Length; i = i + 2)
            {
                scores.Add(float.Parse(sc[i]));
            }
            return scores.ToArray();
        }

        public static List<Round> ParseLog(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != "log")
            {
                throw new System.Exception();
            }
            reader.Read(); 
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new System.Exception();
            }
            reader.Read();

            var rounds = new List<Round>();
            while (reader.TokenType == JsonToken.StartArray)
            {
                rounds.Add(ParseRound(reader));
            }
            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            return rounds;
        }

        public static Round ParseRound(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            var round = new Round();
            var roundInfo = ParseIntArray(reader);
            round.RoundNumber = roundInfo[0];
            round.NbHonbas = roundInfo[1];
            round.NbRiichi = roundInfo[2];
            round.StartingScores = ParseIntArray(reader);
            round.Doras = ParseIntArray(reader);
            round.UraDoras = ParseIntArray(reader);
            var haipai1 = ParseStrArray(reader);
            var draws1 = ParseStrArray(reader);
            var discards1 = ParseStrArray(reader);
            var haipai2 = ParseStrArray(reader);
            var draws2 = ParseStrArray(reader);
            var discards2 = ParseStrArray(reader);
            var haipai3 = ParseStrArray(reader);
            var draws3 = ParseStrArray(reader);
            var discards3 = ParseStrArray(reader);
            var haipai4 = ParseStrArray(reader);
            var draws4 = ParseStrArray(reader);
            var discards4 = ParseStrArray(reader);
            round.HaiPais = new string[][] { haipai1, haipai2, haipai3, haipai4 };
            round.Draws = new string[][] { draws1, draws2, draws3, draws4 };
            round.Discards = new string[][] { discards1, discards2, discards3, discards4 };
            round.Result = parseRoundResults(reader);
            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            return round;
        }

        public static RoundResult[] parseRoundResults(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            var resultName = (string)reader.Value;
            reader.Read();
            var results = new List<RoundResult>();
            while (reader.TokenType == JsonToken.StartArray)
            {
                results.Add(ParseRoundResult(reader, resultName));
            }
            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            return results.ToArray();
        }

        public static RoundResult ParseRoundResult(JsonTextReader reader, string name)
        {
            var result = new RoundResult();
            result.Name = name;
            if (reader.TokenType == JsonToken.StartArray)
            {
                result.Payments = ParseIntArray(reader);
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                result.Winner = ParseIntValue(reader);
                result.Loser = ParseIntValue(reader);
                result.LoserPao = ParseIntValue(reader);
                result.HandScore = ParseStrValue(reader);
                var yakus = new List<string>();
                while (reader.TokenType == JsonToken.String)
                {
                    yakus.Add(ParseStrValue(reader));
                }
                result.Yakus = yakus.ToArray();
                if (reader.TokenType != JsonToken.EndArray)
                {
                    throw new System.Exception();
                }
                reader.Read();
            }
            return result;
        }

        public static Rule ParseRule(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != "rule")
            {
                throw new System.Exception();
            }
            reader.Read();
            reader.Read();
            var rule = new Rule();
            rule.Disp = ParseStrProp("disp", reader);
            rule.Aka53 = ParseIntProp("aka53", reader);
            rule.Aka52 = ParseIntProp("aka52", reader);
            rule.Aka51 = ParseIntProp("aka51", reader);
            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new System.Exception();
            }
            reader.Read();
            return rule;
        }

        public static int[] ParseIntArray(JsonTextReader reader, string name = null)
        {
            if (name != null)
            {
                ParsePropName(name, reader);
            }
            var list = new List<int>();
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            while(reader.TokenType != JsonToken.EndArray)
            {
                list.Add((int)(long)reader.Value);
                reader.Read();
            }
            reader.Read();
            return list.ToArray();
        }
        public static string[] ParseStrArray(JsonTextReader reader, string name = null)
        {
            if(name != null)
            {
                ParsePropName(name,reader);
            }
            var list = new List<string>();
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new System.Exception();
            }
            reader.Read();
            while (reader.TokenType != JsonToken.EndArray)
            {
                list.Add(reader.Value.ToString());
                reader.Read();
            }
            reader.Read();
            return list.ToArray();
        }
        public static void ParsePropName(string name, JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != name)
            {
                throw new System.Exception();
            }
            reader.Read();
        }
        public static string ParseStrProp(string key, JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != key)
            {
                throw new System.Exception();
            }
            reader.Read();
            return ParseStrValue(reader);
        }
        public static int ParseIntProp(string key, JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != key)
            {
                throw new System.Exception();
            }
            reader.Read();
            return ParseIntValue(reader);
        }
        public static int ParseIntValue(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new System.Exception();
            }
            var value = (int)(long)reader.Value;
            reader.Read();
            return value;
        }
        public static string ParseStrValue(JsonTextReader reader)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new System.Exception();
            }
            var value = reader.Value.ToString();
            reader.Read();
            return value;
        }
    }
}
