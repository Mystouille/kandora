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
        public NanikiruProblem NextProblem()
        {
            int index = Random.Next(Problems.Count);
            return Problems[index];
        }
    }
}
