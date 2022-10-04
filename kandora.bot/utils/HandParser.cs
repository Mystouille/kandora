using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.resources;
using System;
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
            var tileList = SplitTiles(hand);
            if (sorted)
            {
                tileList.Sort(new TileComparer());
            }
            return tileList.Select(x => GetEmojiCode(x, client));
        }

        //input: RRRg output: 777z
        public static string GetSimpleHand(string hand)
        {
            var tileList = string.Join("", SimpleTiles(hand));
            return tileList;
        }


        //Recursively builds the hand
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
                        string tileToAdd = $"{hand[j]}{fixedChar}";

                        if (!(isUnique && tiles.Contains(tileToAdd)))
                            tiles.Add(tileToAdd);
                    }
                    k = i + 1; // char after the letter for the next iteration
                }
                i++;
            }
            return tiles;
        }

        //input: RRRg output: 7z,7z,7z
        public static List<string> SimpleTiles(string handWithMelds)
        {
            var tileNames = new List<string>();
            int i = 0;
            var hand = handWithMelds.Replace("'", string.Empty).Replace("k", string.Empty);
            while (i < hand.Length)
            {
                if (SUIT_NAMES.Contains(hand[i])) break;
                i++;
            }
            if (i == hand.Length)
            {
                return tileNames;
            }
            var suit = hand[i];
            var newSuit = suit;
            var subHandValues = hand.Substring(0, i);
            foreach (char c in subHandValues)
            {
                if (c == ' ') continue;
                var tile = $"{c}{suit}";
                if (ALTERNATIVE_IDS.ContainsKey(tile))
                {
                    tile = ALTERNATIVE_IDS[tile];
                    newSuit = tile[1];
                    tile = tile[0].ToString();
                }
                tileNames.Add(tile);
            }
            tileNames.Add(newSuit.ToString());
            if (i == hand.Length - 1)
            {
                return tileNames;
            }
            var restHand = hand.Substring(i + 1);
            tileNames.AddRange(SimpleTiles(restHand));

            return tileNames;
        }

        //input: RRRg output: 7z,7z,7z
        public static List<string> VisualTiles(string hand)
        {
            var tileNames = new List<string>();
            int i = 0;
            while (i < hand.Length)
            {
                if (SUIT_NAMES.Contains(hand[i])) break;
                i++;
            }
            if (i == hand.Length)
            {
                return tileNames;
            }
            var suit = hand[i];
            var subHandValues = hand.Substring(0, i);
            foreach (char c in subHandValues)
            {
                if (c == ' ') continue;
                var tile = $"{c}{suit}";
                if (ALTERNATIVE_IDS.ContainsKey(tile))
                {
                    tile = ALTERNATIVE_IDS[tile];
                    suit = tile[1];
                    tile = tile[0].ToString();
                }
                tileNames.Add(tile);
            }
            tileNames.Add(suit.ToString());
            if (i == hand.Length - 1)
            {
                return tileNames;
            }
            var restHand = hand.Substring(i + 1);
            tileNames.AddRange(VisualTiles(restHand));

            return tileNames;
        }
    }

}
