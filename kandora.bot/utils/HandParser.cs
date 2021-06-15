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

        public static IEnumerable<DiscordEmoji> GetHandEmojiCodes(string hand, DiscordClient client)
        {
            StringBuilder sb = new StringBuilder();
            var tileList = SplitTiles(hand);
            return tileList.Select(x => GetEmojiCode(x, client));
        }

        //input: RRRg output: 777z
        public static string GetSimpleHand(string hand)
        {
            var tileList = string.Join("",SimpleTiles(hand));
            return tileList;
        }


        //Recursively builds the hand
        private static List<string> SplitTiles(string hand)
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
            var subHandValues = hand.Substring(0, i);
            foreach (char c in subHandValues)
            {
                if (c == ' ') continue;
                tileNames.Add($"{c}{hand[i]}");
            }
            if (i == hand.Length - 1)
            {
                return tileNames;
            }
            var restHand = hand.Substring(i + 1);
            tileNames.AddRange(SplitTiles(restHand));

            return tileNames;
        }

        //input: RRRg output: 7z,7z,7z,z
        public static List<string> SimpleTiles(string hand)
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


    }

}