using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace kandora.bot.utils
{
    public static class HandParser
    {
        private static readonly HashSet<char> SUIT_NAMES = new HashSet<char>() { 'p', 'm', 's', 'z', 'w', 'd' };

        private static Dictionary<string, string> ALTERNATIVE_IDS = new Dictionary<string, string>()
            {
            {"Ew","1z"},
            {"Sw","2z"},
            {"Ww","3z"},
            {"Nw","4z"},
            {"1w","1z"},
            {"2w","2z"},
            {"3w","3z"},
            {"4w","4z"},
            {"Wd","5z"},
            {"Gd","6z"},
            {"Rd","7z"},
            {"1d","5z"},
            {"2d","6z"},
            {"3d","7z"}
        };

        public static DiscordEmoji GetEmojiCode(string tileName, DiscordClient client)
        {
            try
            {
                return DiscordEmoji.FromName(client, ":" + tileName + ":");
            }
            catch
            {
                if (ALTERNATIVE_IDS.ContainsKey(tileName))
                {
                    return GetEmojiCode(ALTERNATIVE_IDS.GetValueOrDefault(tileName), client);
                }
                return null;
            }
        }

        public static IEnumerable<DiscordEmoji> GetHandEmojiCodes(string hand, DiscordClient client, bool sorted = false)
        {
            StringBuilder sb = new StringBuilder();
            var tileList = SplitTiles(hand, true); // We want unique emotes
            if (sorted)
            {
                tileList.Sort(new TileComparer());
            }
            return tileList.Select(x => GetEmojiCode(x, client));
        }

        // Builds the hand
        // ex. 123456m556677p99s -> 1m2m3m4m5m6m5p5p6p6p7p7p9s9s
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
                        string tileToAdd = SimplifyTile($"{hand[j]}{fixedChar}");

                        if (!(isUnique && tiles.Contains(tileToAdd)))
                            tiles.Add(tileToAdd);
                    }
                    k = i + 1; // char after the letter for the next iteration
                }
                i++;
            }
            return tiles;
        }

        // Joins the SplitTiles return in one string 
        public static string GetSimpleHand(string hand)
        {
            var tileList = string.Join("", SplitTiles(hand));
            return tileList;
        }

        public static string SimplifyTile(string tile)
        {
            return ALTERNATIVE_IDS.ContainsKey(tile) ? ALTERNATIVE_IDS[tile] : tile;
        }
    }
}
