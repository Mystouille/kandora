using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.tests.HandParserTests;

public class SimplifyTileTests
{
    [Theory]
    [InlineData("1s", "1s")]
    [InlineData("3p", "3p")]
    [InlineData("5z", "5z")]
    [InlineData("9e", "9e")]
    [InlineData("iiiii", "iiiii")]
    [InlineData("1z8p", "1z8p")]
    [InlineData("test", "test")]
    public void WhenSimpleValues_ShouldReturnTheSame(string entry, string expectedOutput)
    {
        string returnedValue = HandParser.SimplifyTile(entry);

        Assert.Equal(expectedOutput, returnedValue);
    }

    [Theory]
    [InlineData("Ew", "1z")]
    [InlineData("Nw", "4z")]
    [InlineData("1w", "1z")]
    [InlineData("4w", "4z")]
    [InlineData("Wd", "5z")]
    [InlineData("Rd", "7z")]
    [InlineData("1d", "5z")]
    [InlineData("3d", "7z")]
    public void WhenAlternateValues_ShouldReturnSimpleValues(string entry, string expectedOutput)
    {
        string returnedValue = HandParser.SimplifyTile(entry);

        Assert.Equal(expectedOutput, returnedValue);
    }
}
