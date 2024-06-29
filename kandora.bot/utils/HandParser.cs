using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace kandora.bot.utils
{
    public static class HandParser
    {
        public static readonly HashSet<char> SUIT_NAMES = new HashSet<char>() { 'p', 'm', 's', 'z' };

        public static DiscordEmoji GetEmojiCode(string tileName, DiscordClient client)
        {
            return DiscordEmoji.FromName(client, ":" + tileName + ":");
        }

        public static IEnumerable<DiscordEmoji> GetHandEmojiCodes(string hand, DiscordClient client, bool sorted = false)
        {
            StringBuilder sb = new StringBuilder();
            var tileList = SplitTiles(hand); // We want unique emotes
            if (sorted)
            {
                tileList.Sort(new TileComparer());
            }
            return tileList.Select(x => GetEmojiCode(x, client));
        }

        // Builds the hand
        // ex. 12345'6m556677'p99s -> 1m2m3m4m5m'6m5p5p6p6p7p7p'9s9s
        // ex. unique : 11123455s -> 1s2s3s4s5s
        public static List<string> SplitTiles(string hand, bool isUnique = false)
        {
            var tiles = new List<string>();

            int i = 0;
            int k = 0;
            while (i < hand.Length)
            {
                if (SUIT_NAMES.Contains(hand[i]))
                {
                    char fixedChar = hand[i];
                    for (int j = k; j < i; j++)
                    {
                        var tileNumber = hand[j];
                        if (!char.IsDigit(tileNumber))
                        {
                            continue;
                        }
                        var called = "";
                        if (hand[j + 1] == '\'')
                        {
                            called += '\'';
                            j++;
                        }
                        string tileToAdd = $"{tileNumber}{called}{fixedChar}";

                        tiles.Add(tileToAdd);
                    }
                    k = i + 1; // char after the letter for the next iteration
                }
                else if (!char.IsDigit(hand[i]) && hand[i]!= '\'')
                {
                    k = i + 1;
                }
                i++;
            }
            return tiles;
        }

        // Joins the SplitTiles return in one string 
        public static string[] GetSimpleHand(string hand)
        {
            var handAndMeld = hand.Split(' ');
            var closedHand = handAndMeld[0];
            var splitClosedHand = string.Join("", SplitTiles(closedHand));
            var melds = handAndMeld.Count() > 1 ? handAndMeld[1]: "";
            var splitMelds = string.Join("", SplitTiles(melds));

            return new[] { splitClosedHand, splitMelds} ;
        }
    }
}
