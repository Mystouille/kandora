using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class ProblemSeries
    {
        public ProblemSeries() { }

        public OngoingQuizz OngoingProblem { get; }
        public int NbProblems { get; }
        public int NbRemainingProblems { get; private set; }

    }
}
