using kandora.bot.utils;
using System;
using System.IO;
using System.Collections.Generic;
using DSharpPlus;
using System.Linq;
using kandora.bot.mahjong;
using kandora.bot.resources;

namespace kandora.bot.services.discord.problems
{
    class ChinitsuQuizzGenerator: IQuizzGenerator
    {
        private string SuitParam { get; }
        private DiscordClient Client { get; }

        public ChinitsuQuizzGenerator(DiscordClient client, string suitParam){
            SuitParam = suitParam;
            Client = client;
        }

        public MultipleChoicesQuestion GetNewQuestion()
        {
            var suit = SuitParam;
            if (SuitParam == "r" || SuitParam == "")
            {
                var suits = new string[] { "s", "m", "p" };
                var random = new Random();
                suit = suits[random.Next(3)];
            }
            else if (SuitParam.Length > 1 || (SuitParam != "s" && SuitParam != "m" && SuitParam != "p"))
            {
                suit = "s";
            }

            var (fileStream, handStr) = GetRandomChinitsuTenpai(suit);
            var waits = GetWaits(handStr);
            var optionEmojis = HandParser.GetHandEmojiCodes($"123456789{suit}", Client).Distinct();
            var answerEmojis = HandParser.GetHandEmojiCodes(waits, Client).Distinct();
            return new MultipleChoicesQuestion(
                image: fileStream,
                message: Resources.quizz_fullflush_questionMessage,
                messageWithTimeout: Resources.quizz_fullflush_questionMessageWithTime,
                optionEmojis: optionEmojis,
                answerEmojis: answerEmojis
            );
        }

        private (FileStream,string) GetRandomChinitsuTenpai(string suit)
        {
            var offset = 0;
            if (suit == "s")
            {
                offset = 18;
            }
            else if (suit == "p")
            {
                offset = 9;
            }
            var rd = new Random();
            int[] hand;
            string handStr = "";
            int shanten = 7;
            bool handAlreadyExist = true;
            var nbIter = 0;
            var shantenCalc = new ShantenCalculator();
            while (shanten != 0 || handAlreadyExist)
            {
                var availableTiles = new List<int>();
                for (int i = 0; i <= 8; i++)
                {
                    availableTiles.Add(i);
                    availableTiles.Add(i);
                    availableTiles.Add(i);
                    availableTiles.Add(i);
                }
                hand = new int[34];
                for (int i = 1; i <= 13; i++)
                {
                    var roll = rd.Next(0, availableTiles.Count);
                    var value = availableTiles[roll];
                    hand[offset + value]++;
                    availableTiles.RemoveAt(roll);
                }
                shanten = shantenCalc.GetNbShanten(hand);
                nbIter++;
                var hand136 = TilesConverter.From34countTo136(hand.ToList());
                handStr = TilesConverter.ToString(hand136);

                handAlreadyExist = ImageToolbox.ImageExists(handStr);
            }
            return (ImageToolbox.GetImageFromTiles(handStr),handStr);
        }

        /// <summary>
        /// Lists all the waits of a tenpai chinitsu hand
        /// </summary>
        /// <param name="handStr">The hand in string format(</param>
        /// <param name="suit">the suit of the hand</param>
        /// <returns>List of 34idx format waits</returns>
        private string GetWaits(string handStr)
        {
            var sc = new ShantenCalculator();

            var hand34 = TilesConverter.FromStringTo34Count(handStr);
            if(sc.GetNbShanten(hand34) != 0)
            {
                return null;
            }

            var suit = handStr[handStr.Length - 1];

            var offset = 0;
            if (suit == 's')
            {
                offset = 18;
            }
            else if (suit == 'p')
            {
                offset = 9;
            }

            var result = "";
            for (int i = 0; i<=8; i++)
            {
                var index34 = offset + i;
                hand34[index34]++;
                if(sc.GetNbShanten(hand34) == Constants.AGARI_STATE && hand34[index34]<=4)
                {
                    result += (i + 1);
                }
                hand34[index34]--;
            }

            return result+suit;
        }
    }
}
