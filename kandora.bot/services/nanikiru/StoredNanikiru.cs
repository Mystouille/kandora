using kandora.bot.mahjong;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.nanikiru
{
    public sealed class StoredNanikiru
    {
        private Random Random = new Random();
        private StoredNanikiru() {
            Problems = NanikiruParser.ParseNanikiruProblems();
            UzakuProblems = Problems.Where(problem => problem.Source.StartsWith("300-") || problem.Source.StartsWith("301-")).ToList();
            RemainingUzakuProblems = UzakuProblems.Count > 0 ? Enumerable.Range(0, (UzakuProblems.Count/3) -1).ToList() : new List<int>();
        }
        private static StoredNanikiru instance = null;
        public static StoredNanikiru Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StoredNanikiru();
                }
                return instance;
            }
        }

        public List<NanikiruProblem> Problems { get; set; }
        public List<NanikiruProblem> UzakuProblems { get; set; }
        public List<int> RemainingUzakuProblems { get; set; }
        public NanikiruProblem NextProblem()
        {
            int index = Random.Next(Problems.Count);
            return Problems[index];
        }
        public List<NanikiruProblem> NextUzakuPage()
        {
            if (RemainingUzakuProblems.Count == 0)
            {
                RemainingUzakuProblems = Enumerable.Range(0, (UzakuProblems.Count/3)).ToList();
            }
            int startIndex = RemainingUzakuProblems.ElementAt(Random.Next(RemainingUzakuProblems.Count));
            RemainingUzakuProblems.Remove(startIndex);
            return UzakuProblems.GetRange(startIndex*3, 3);
        }
    }
}
