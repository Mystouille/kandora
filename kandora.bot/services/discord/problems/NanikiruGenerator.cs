using kandora.bot.utils;
using System.Collections.Generic;
using DSharpPlus;
using System.Linq;
using kandora.bot.services.nanikiru;
using kandora.bot.resources;
using DSharpPlus.Entities;
using System.Text;
using kandora.bot.mahjong;
using Microsoft.VisualBasic;

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
                var basicHand = HandParser.GetSimpleHand(problem.Hand);
                var closedHand = basicHand[0];
                var melds = basicHand[1];

                var optionEmojis = HandParser.GetHandEmojiCodes(closedHand, Client).ToList();

                var source = problem.Source;

                var answerEmojis = new List<DiscordEmoji> { HandParser.GetEmojiCode(problem.Answer.Substring(0, 2), Client) };
                var riichiEmoji = DiscordEmoji.FromName(Client, Reactions.Riichi);
                var kanEmoji = DiscordEmoji.FromName(Client, Reactions.Kan);
                if (problem.Answer.Length > 2 && problem.Answer[2] == 'r')
                {
                    answerEmojis.Add(riichiEmoji);
                    optionEmojis.Add(riichiEmoji);
                }
                else
                {
                    var shantenCalc = new ShantenCalculator();
                    var shanten = shantenCalc.GetNbShanten(TilesConverter.FromStringTo34Count(closedHand));
                    if (melds.Length == 0 && shanten == 0)
                    {
                        optionEmojis.Add(riichiEmoji);
                    }
                }
                if (problem.Answer.Length > 2 && problem.Answer[2] == 'k')
                {
                    answerEmojis.Add(kanEmoji);
                    optionEmojis.Add(kanEmoji);
                }
                else
                {
                    var tiles = TilesConverter.FromStringTo34Count(closedHand);
                    if(tiles.Where(x=> x == 4).Count() > 0) {
                        optionEmojis.Add(kanEmoji);
                    }
                }

                if (!withTimeout)
                {
                    optionEmojis.Add(DiscordEmoji.FromName(Client, Reactions.EYES));
                }


                var stream = ImageToolbox.GetImageFromTiles(closedHand, melds, separateLastTile: true);
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
                    hand: problem.Hand,
                    doras: problem.Dora,
                    ukeire: problem.Ukeire,
                    source: problem.Source,
                    explanation: explanation,
                    optionEmojis: optionEmojis,
                    answerEmojis: answerEmojis,
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
