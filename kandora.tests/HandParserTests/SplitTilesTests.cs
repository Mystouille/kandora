﻿using kandora.bot.utils;

namespace handora.tests.HandParserTests;

public class SplitTileTests
{
    [Theory]
    [InlineData("111222333p44455s", "1p2p3p4s5s")]
    [InlineData("146s89p2227m234s55z", "1s4s6s8p9p2m7m2s3s5z")]
    [InlineData("26z987s123456z66p7s", "2z6z9s8s7s1z3z4z5z6p")]
    [InlineData("54s898p12365s897p1z", "5s4s8p9p1s2s3s6s7p1z")]
    [InlineData("abcefghs", "asbscsesfsgshs")]
    [InlineData("123sp45z123s", "1s2s3s4z5z")]
    [InlineData("123s123k", "1s2s3s")]
    [InlineData("123k", "")]
    [InlineData("123d222w154sEEEw", "5z6z7z2z1s5s4s1z")]
    [InlineData("RRRd", "7z")]
    public void WhenMethodUsedWithUnique_ShouldReturnHandParsed(string hand, string expectedOutput)
    {
        List<string> tiles = HandParser.SplitTiles(hand, true);

        Assert.Equal(expectedOutput, string.Join("", tiles));
    }

    [Fact(Timeout = 10)]
    public void WhenLargeString_ShouldBeExecutedQuickly()
    {
        string tileList = new string('1', 3000) + 's';
        List<string> tiles = HandParser.SplitTiles(tileList, true);

        Assert.Equal("1s", string.Join("", tiles));
    }

    [Theory]
    [InlineData("111222333p44455s", "1p1p1p2p2p2p3p3p3p4s4s4s5s5s")]
    [InlineData("146s89p2227m234s55z", "1s4s6s8p9p2m2m2m7m2s3s4s5z5z")]
    [InlineData("26z987s123456z66p7s", "2z6z9s8s7s1z2z3z4z5z6z6p6p7s")]
    [InlineData("54s898p12365s897p1z", "5s4s8p9p8p1s2s3s6s5s8p9p7p1z")]
    [InlineData("abcefghs", "asbscsesfsgshs")]
    [InlineData("123sp45z123s", "1s2s3s4z5z1s2s3s")]
    [InlineData("123s123k", "1s2s3s")]
    [InlineData("123k", "")]
    [InlineData("123s", "1s2s3s")]
    [InlineData("123d222w154sEEEw", "5z6z7z2z2z2z1s5s4s1z1z1z")]
    [InlineData("RRRd", "7z7z7z")]
    public void WhenMethodUsedWithoutUnique_ShouldReturnHandParsed(string hand, string expectedOutput)
    {
        List<string> tiles = HandParser.SplitTiles(hand, false);

        Assert.Equal(expectedOutput, string.Join("", tiles));
    }
}