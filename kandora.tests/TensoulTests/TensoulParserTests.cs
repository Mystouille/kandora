using kandora.bot.models;
using kandora.bot.utils;
using kandora.bot.utils.TensoulLogParser;

namespace kandora.tests.TenhouTests;

public class TensoulParserTests
{
    string gameOne;
    string gameTwo;

    public TensoulParserTests()
    {
        gameOne = File.ReadAllText("TensoulTests\\tensoulLog.json");
        gameTwo = File.ReadAllText("TensoulTests\\tensoulLog2.json");
    }

    [Fact]
    public void TensoulParser_GivenStandardJson_ShouldBeSuccessful()
    {
        var riichiGame = TensoulParser.ParseTensoulFormatGame(gameOne, GameType.Tenhou);

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
    public void TensoulParser_GivenDoubleRon_ShouldHaveMultipleResults()
    {
        var riichiGame = TensoulParser.ParseTensoulFormatGame(gameTwo, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Equal(2, riichiGame.Rounds[1].Result.Count());
    }

    [Fact]
    public void TensoulParser_GivenOyaWinning_ShouldHaveIncrementedHonba()
    {
        var riichiGame = TensoulParser.ParseTensoulFormatGame(gameTwo, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Equal(0, riichiGame.Rounds[1].NbHonbas);
        Assert.Equal(1, riichiGame.Rounds[2].NbHonbas);
    }

    [Fact]
    public void TensoulParser_GivenDraw_ShouldHaveNoWinnerAndIncrementedHonba()
    {
        var riichiGame = TensoulParser.ParseTensoulFormatGame(gameTwo, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Single(riichiGame.Rounds[3].Result);
        Assert.Equal(new int[4] { -1500, 1500, 1500, -1500 }, riichiGame.Rounds[3].Result[0].Payments);
        Assert.Equal(2, riichiGame.Rounds[3].NbHonbas);
        Assert.Equal(3, riichiGame.Rounds[4].NbHonbas);
    }

    [Fact]
    public void TensoulParser_GivenRon_ShouldHaveProperPayments()
    {
        var riichiGame = TensoulParser.ParseTensoulFormatGame(gameTwo, GameType.Tenhou);

        Assert.NotNull(riichiGame);
        Assert.Equal(new int[4] {0, 0, -8000, 9000}, riichiGame.Rounds[1].Result[0].Payments);
        Assert.Equal(new int[4] { 0, 18000, -18000, 0 }, riichiGame.Rounds[1].Result[1].Payments);
    }
}
