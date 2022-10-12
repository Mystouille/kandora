using kandora.bot.models;

namespace handora.tests.Models;

public class GameTests
{
    [Theory]
    [InlineData(30000, 30000, 30000, 30000, "1234", "1234", "1234", "1234")]
    [InlineData(32000, 32000, 28000, 28000, "12", "12", "34", "34")]
    [InlineData(32000, 32000, 29000, 27000, "12", "12", "3", "4")]
    [InlineData(34000, 31000, 31000, 24000, "1", "23", "23", "4")]
    [InlineData(34000, 33000, 30000, 23000, "1", "2", "3", "4")]
    public void When4PlayerScores_ShouldHaveCorrectPlacement(int score1, int score2, int score3, int score4, string place1, string place2, string place3, string place4)
    {
        var server = new Server("serverId", "server", "", "", 1);
        var game = new Game("logId", server, "", "", "", "", GameType.IRL, DateTime.Now, false);
        game.User1Score = score1;
        game.User2Score = score2;
        game.User3Score = score3;
        game.User4Score = score4;

        Assert.Equal(game.User1Placement, place1);
        Assert.Equal(game.User2Placement, place2);
        Assert.Equal(game.User3Placement, place3);
        Assert.Equal(game.User4Placement, place4);
    }
}