using kandora.bot.utils;
using System.Collections.Generic;
using DSharpPlus;
using System.Linq;
using kandora.bot.services.nanikiru;
using kandora.bot.resources;
using DSharpPlus.Entities;
using System.Text;

namespace kandora.bot.services.discord.problems
{
    public class NanikiruGenerator
    {
        private bool UzakuStyle { get; }
        private DiscordClient Client { get; }
        private List<NanikiruQuestion> QuestionGroup { get; set; }
        private int Index { get; set; }
        private bool WithTimeout { get; set; }

        public NanikiruGenerator(DiscordClient client, bool uzakuStyle, bool withTimeout){
            UzakuStyle = uzakuStyle;
            Client = client;
            WithTimeout = withTimeout;
            QuestionGroup = GetNewQuestionGroup(withTimeout);
            Index = 0;
        }

        private List<NanikiruQuestion> GetNewQuestionGroup(bool withTimeout)
        {

            var problemGroup = new List<NanikiruProblem>();
            if (UzakuStyle)
            {
                problemGroup = StoredNanikiru.Instance.NextUzakuPage();
            }
            else
            {
                problemGroup = new List<NanikiruProblem> { StoredNanikiru.Instance.NextProblem() };
            }

            var questions = new List<NanikiruQuestion>();

            foreach (var problem in problemGroup)
            {
                var optionEmojis = HandParser.GetHandEmojiCodes(problem.Hand, Client).Distinct();
                if (!withTimeout) {
                    optionEmojis = optionEmojis.Append(DiscordEmoji.FromName(Client, Reactions.EYES));
                }
                var answerEmoji = HandParser.GetEmojiCode(problem.Answer, Client);
                var stream = ImageToolbox.GetImageFromTiles(problem.Hand, separateLastTile: true);
                var explanation = problem.ExplanationEng;
                if(Resources.cultureInfoStr == "fr-FR" && problem.ExplanationFr.Length > 0)
                {
                    // todo fix encoding problem
                    //explanation = problem.ExplanationFr; 
                    explanation = problem.ExplanationEng; 
                }

                var sb = new StringBuilder();
                sb.AppendLine("Manche: " + problem.Round);
                sb.AppendLine("Vent joueur: " + problem.Seat);
                sb.AppendLine("Tour: " + problem.Turn);
                sb.AppendLine("Dora: " + HandParser.GetEmojiCode(problem.Dora, Client));
                var header = sb.ToString();

                questions.Add(new NanikiruQuestion(
                    image: stream,
                    ukeire: problem.Ukeire,
                    source: problem.Source,
                    explanation: explanation,
                    optionEmojis: optionEmojis,
                    answerEmoji: answerEmoji,
                    message: Resources.quizz_nanikiru_questionMessage + "\n" + header,
                    messageWithTimeout: Resources.quizz_nanikiru_questionMessageWithTime + "\n" + header
                ));
            }
            return questions;
        }

        public NanikiruQuestion GetNewQuestion()
        {
            if(Index >= QuestionGroup.Count)
            {
                QuestionGroup = GetNewQuestionGroup(WithTimeout);
                Index = 0;
            }
            return QuestionGroup[Index++];
        }
    }
}
