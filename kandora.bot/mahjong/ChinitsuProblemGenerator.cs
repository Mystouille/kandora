using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace kandora.bot.mahjong
{
    class ChinitsuProblemGenerator
    {
        public static (FileStream, ISet<int>, string) getNewProblem(string suit = "s")
        {
            var (fileStream, handStr) = ImageToolbox.getNewProblem(suit);

            return (fileStream, GetWaits(handStr), "123456789"+suit);
            
        }

        /// <summary>
        /// Lists all the waits of a tenpai chinitsu hand
        /// </summary>
        /// <param name="handStr">The hand in string format(</param>
        /// <param name="suit">the suit of the hand</param>
        /// <returns>List of 34idx format waits</returns>
        public static ISet<int> GetWaits(string handStr)
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

            var result = new HashSet<int>();
            for (int i = 0; i<=8; i++)
            {
                var index34 = offset + i;
                hand34[index34]++;
                if(sc.GetNbShanten(hand34) == Constants.AGARI_STATE)
                {
                    result.Add(index34);
                }
                hand34[index34]--;
            }

            return result;
        }
    }
}
