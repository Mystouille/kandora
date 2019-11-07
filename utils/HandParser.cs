using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Text;


namespace Kandora
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
            {"Rd","5z"},
            {"Wd","6z"},
            {"Gd","7z"},
            {"1d","5z"},
            {"2d","6z"},
            {"3d","7z"}
        };

        public static string GetEmojiCode(string tileName, DiscordClient client)
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
                return "";
            }
        }

        public static string GetHandEmojiCode(string hand, DiscordClient client)
        {
            StringBuilder sb = new StringBuilder();
            var tileList = ParseHand(hand);
            foreach (var tile in tileList)
            {
                sb.Append(GetEmojiCode(tile, client));
            }

            return sb.ToString();
        }


        //Recursively builds the hand
        private static List<string> ParseHand(string hand)
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
            tileNames.AddRange(ParseHand(restHand));

            return tileNames;
        }

    }

}