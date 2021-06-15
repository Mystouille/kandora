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
            var (fileStream, handStr) = ImageToolbox.getNewProblem("s");

            return (fileStream, GetWaits(handStr, suit), "123456789"+suit);
            
        }

        // output: list of 34-indices
        public static ISet<int> GetWaits(string hand, string suit)
        {
            var sc = new ShantenCalculator();

            var hand34 = TilesConverter.one_line_string_to_34_array(hand);
            if(sc.Calculate_shanten(hand34) != 0)
            {
                return null;
            }

            var offset = 0;
            if (suit == "s")
            {
                offset = 18;
            }
            else if (suit == "p")
            {
                offset = 9;
            }

            var result = new HashSet<int>();
            for (int i = 0; i<=8; i++)
            {
                var index34 = offset + i;
                hand34[index34]++;
                if(sc.Calculate_shanten(hand34) == Constants.AGARI_STATE)
                {
                    result.Add(index34);
                }
                hand34[index34]--;
            }

            return result;
        }
    }
}
