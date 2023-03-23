using kandora.bot.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.nanikiru
{
    public class NanikiruProblem
    {
        public string Round { get; set; }
        public string Seat { get; set; }
        public string Turn { get; set; }
        public string Dora { get; set; }
        public string Hand { get; set; }
        public string Answer { get; set; }
        public string ExplanationEng { get; set; }
        public string ExplanationFr { get; set; }
        public string Source { get; set; }
    }
}
