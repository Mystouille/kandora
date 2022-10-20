using kandora.bot.models;
using kandora.bot.utils;
using kandora.bot.utils.TenhouParser;

namespace kandora.tests.TenhouTests;

public class TenhouParserTests
{
    [Fact]
    public void TenhouLogParser_GivenStandardJson_ShouldBeSuccessful()
    {
        string json = File.ReadAllText("TenhouTests\\tenhouLog.json");

        var riichiGame = TenhouLogParser.ParseTenhouFormatGame(json, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Equal(new float[4] { -16.8f, 37.4f, 14.1f, -34.7f }, riichiGame.FinalRankDeltas);
        Assert.Equal(new int[4] { 13200, 47400, 34100, 5300 }, riichiGame.FinalScores);
        Assert.Equal(new string[4] { "闹钟妹", "PDGON", "落第生きらら", "あろまま" }, riichiGame.Names);
        Assert.Equal(new float[4] { 2002, 1320, 1728, 2927 }, riichiGame.Rate);
        Assert.Equal(new string[4] { "89852", "72011596", "75144431", "75115153" }, riichiGame.UserIds);
        Assert.Equal(new string[4] { "雀聖★1", "-", "雀聖★1", "雀聖★2" }, riichiGame.Dan);
        Assert.Equal(1, riichiGame.Rule.Aka51);
        Assert.Equal(1, riichiGame.Rule.Aka52);
        Assert.Equal(1, riichiGame.Rule.Aka53);
        Assert.Equal(15, riichiGame.Rounds.Count);
        Assert.Equal(14, riichiGame.Rounds[0].Discards[0].Length);
        Assert.Equal(14, riichiGame.Rounds[0].Draws[0].Length);
        Assert.Equal("20符3飜700-1300点", riichiGame.Rounds[0].Result[0].HandScore);
        Assert.Equal(new int[4] { -1300, 3700, -700, -700 }, riichiGame.Rounds[0].Result[0].Payments);
        Assert.Equal(new string[4] { "門前清自摸和(1飜)", "平和(1飜)", "立直(1飜)", "裏ドラ(0飜)" }, riichiGame.Rounds[0].Result[0].Yakus);
        Assert.Equal(new int[4] { 23700, 27700, 24300, 24300 }, riichiGame.Rounds[1].StartingScores);
        Assert.Equal("2.3", riichiGame.Ver);
    }

    [Fact]
    public void TenhouLogParserNew_GivenStandardJson_ShouldBeSuccessful()
    {
        string json = File.ReadAllText("TenhouTests\\tenhouLog.json");

        var riichiGame = TenhouLogParserNew.ParseTenhouFormatGame(json, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Equal("王座の間南喰赤", riichiGame.Title[0]);
        Assert.Equal("Thu, 02 Jun 2022 09:18:52 GMT", riichiGame.Title[1]);
        Assert.Equal(new float[4] { -16.8f, 37.4f, 14.1f, -34.7f }, riichiGame.FinalRankDeltas);
        Assert.Equal(new int[4] { 13200, 47400, 34100, 5300 }, riichiGame.FinalScores);
        Assert.Equal(new string[4] { "闹钟妹", "PDGON", "落第生きらら", "あろまま" }, riichiGame.Names);
        Assert.Equal(new float[4] { 2002, 1320, 1728, 2927 }, riichiGame.Rate);
        Assert.Equal(new string[4] { "89852", "72011596", "75144431", "75115153" }, riichiGame.UserIds);
        Assert.Equal(new string[4] { "雀聖★1", "-", "雀聖★1", "雀聖★2" }, riichiGame.Dan);
        Assert.Equal(1, riichiGame.Rule.Aka51);
        Assert.Equal(1, riichiGame.Rule.Aka52);
        Assert.Equal(1, riichiGame.Rule.Aka53);
        Assert.Equal(15, riichiGame.Rounds.Count);
        Assert.Equal(14, riichiGame.Rounds[0].Discards[0].Length);
        Assert.Equal(14, riichiGame.Rounds[0].Draws[0].Length);
        Assert.Equal("20符3飜700-1300点", riichiGame.Rounds[0].Result[0].HandScore);
        Assert.Equal(new int[4] { -1300, 3700, -700, -700 }, riichiGame.Rounds[0].Result[0].Payments);
        Assert.Equal(new string[4] { "門前清自摸和(1飜)", "平和(1飜)", "立直(1飜)", "裏ドラ(0飜)" }, riichiGame.Rounds[0].Result[0].Yakus);
        Assert.Equal(new int[4] { 23700, 27700, 24300, 24300 }, riichiGame.Rounds[1].StartingScores);
        Assert.Equal("2.3", riichiGame.Ver);
    }

    [Fact]
    public void TenhouLogParserOld_GivenStandardJson_ShouldBeTheSameThanPrevious()
    {
        string json = File.ReadAllText("TenhouTests\\tenhouLog.json");

        var riichiGameOld = TenhouLogParser.ParseTenhouFormatGame(json, GameType.Tenhou);
        var riichiGameNew = TenhouLogParserNew.ParseTenhouFormatGame(json, GameType.Tenhou);

        Assert.Equal(riichiGameOld.FinalRankDeltas, riichiGameNew.FinalRankDeltas);
        Assert.Equal(riichiGameOld.FinalScores, riichiGameNew.FinalScores);
        Assert.Equal(riichiGameOld.Names, riichiGameNew.Names);
        Assert.Equal(riichiGameOld.Rate, riichiGameNew.Rate);
        Assert.Equal(riichiGameOld.UserIds, riichiGameNew.UserIds);
        Assert.Equal(riichiGameOld.Dan, riichiGameNew.Dan);
        Assert.Equal(riichiGameOld.Rule.Aka51, riichiGameNew.Rule.Aka51);
        Assert.Equal(riichiGameOld.Rule.Aka52, riichiGameNew.Rule.Aka52);
        Assert.Equal(riichiGameOld.Rule.Aka53, riichiGameNew.Rule.Aka53);
        Assert.Equal(riichiGameOld.Rounds.Count, riichiGameNew.Rounds.Count);
        Assert.Equal(riichiGameOld.Ver, riichiGameNew.Ver);

        for (int i = 0; i < riichiGameOld.Rounds.Count; i++)
        {
            for(int j = 0; j < riichiGameOld.Rounds[i].Discards.Length; j++)
                Assert.Equal(riichiGameOld.Rounds[i].Discards[j], riichiGameNew.Rounds[i].Discards[j]);
            for (int j = 0; j < riichiGameOld.Rounds[i].HaiPais.Length; j++)
                Assert.Equal(riichiGameOld.Rounds[i].HaiPais[j], riichiGameNew.Rounds[i].HaiPais[j]);
            for (int j = 0; j < riichiGameOld.Rounds[i].Draws.Length; j++)
                Assert.Equal(riichiGameOld.Rounds[i].Draws[j], riichiGameNew.Rounds[i].Draws[j]);
            for (int j = 0; j < riichiGameOld.Rounds[i].Result.Length; j++)
            {
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].HandScore, riichiGameNew.Rounds[i].Result[j].HandScore);
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].Payments, riichiGameNew.Rounds[i].Result[j].Payments);
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].Winner, riichiGameNew.Rounds[i].Result[j].Winner);
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].Loser, riichiGameNew.Rounds[i].Result[j].Loser);
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].LoserPao, riichiGameNew.Rounds[i].Result[j].LoserPao);
                Assert.Equal(riichiGameOld.Rounds[i].Result[j].Yakus, riichiGameNew.Rounds[i].Result[j].Yakus);

            }
            Assert.Equal(riichiGameOld.Rounds[i].StartingScores, riichiGameNew.Rounds[i].StartingScores);
        }
    }
}
