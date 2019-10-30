using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Text;


namespace Kandora
{
    public static class HandParser
    {
        private static Dictionary<string, int> SUIT_SIZES = new Dictionary<string, int>()
            {
            {"p",9},
            {"m",9},
            {"s",9},
            {"z",7}
        };

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
            var found = false;
            while (i < hand.Length)
            {
                switch (hand[i])
                {
                    case 'p': found = true; break;
                    case 'm': found = true; break;
                    case 's': found = true; break;
                    case 'z': found = true; break;
                    case 'w': found = true; break;
                    case 'd': found = true; break;
                }
                if (found) break;
                i++;
            }
            if (i == hand.Length)
            {
                return tileNames;
            }
            var subHandValues = hand.Substring(0, i);
            var subHandSuit = hand[i].ToString();
            foreach (char c in subHandValues)
            {
                if (c == ' ') continue;
                tileNames.Add(c.ToString() + subHandSuit);
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