using CsvHelper;
using kandora.bot.services.nanikiru;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace kandora.bot.utils
{
    public class NanikiruParser
    {
        private static string fileName = "nanikiru.csv";
        public static List<NanikiruProblem> ParseNanikiruProblems()
        {
            using var reader = new StreamReader(FileUtils.GetStreamFromResourceFile(fileName));

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<NanikiruProblem>().ToList();
            }
        }
    }
}
