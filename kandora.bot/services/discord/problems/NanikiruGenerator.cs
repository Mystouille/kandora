using kandora.bot.utils;
using System.Collections.Generic;
using DSharpPlus;
using System.Linq;
using kandora.bot.services.nanikiru;
using kandora.bot.resources;

namespace kandora.bot.services.discord.problems
{
    public class NanikiruGenerator
    {
        private bool UzakuStyle { get; }
        private DiscordClient Client { get; }
        private List<SingleChoiceQuestion> QuestionGroup { get; set; }
        private int Index { get; set; }

        public NanikiruGenerator(DiscordClient client, bool uzakuStyle){
            UzakuStyle = uzakuStyle;
            Client = client;
        }

        private List<SingleChoiceQuestion> GetNewQuestionGroup()
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

            var questions = new List<SingleChoiceQuestion>();

            foreach(var problem in problemGroup)
            {
                var optionEmojis = HandParser.GetHandEmojiCodes(problem.Hand, Client).Distinct();
                var answerEmoji = HandParser.GetEmojiCode(problem.Answer, Client);
                var stream = ImageToolbox.GetImageFromTiles(problem.Hand, separateLastTile: true);
                var explanation = problem.ExplanationEng;
                if(Resources.cultureInfoStr == "fr-FR" && problem.ExplanationFr.Length > 0)
                {
                    explanation = problem.ExplanationFr;
                }
                questions.Add(new SingleChoiceQuestion(
                    image: stream,
                    ukeire: problem.Ukeire,
                    source: problem.Source,
                    explanation: explanation,
                    optionEmojis: optionEmojis,
                    answerEmoji: answerEmoji
                ));
            }
            return questions;
        }

        public SingleChoiceQuestion GetNewQuestion()
        {
            if(Index >= QuestionGroup.Count)
            {
                QuestionGroup = GetNewQuestionGroup();
                Index = 0;
            }
            return QuestionGroup[Index++];
        }
    }
}
