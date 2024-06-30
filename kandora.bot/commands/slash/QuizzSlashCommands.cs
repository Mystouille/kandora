using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using kandora.bot.resources;
using System;
using kandora.bot.services.discord.problems;
using kandora.bot.services.discord;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("quizz", Resources.quizz_groupDescription)]
    class QuizzSlashCommands: KandoraSlashCommandModule
    {
        [SlashCommand("chinitsu", Resources.quizz_fullflush_description)]
        public async Task ChinitsuProblem(InteractionContext ctx,
            [Option(Resources.quizz_options_questions, Resources.quizz_options_questions_description)] long nbRounds = 1,
            [Option(Resources.quizz_option_timeout, Resources.quizz_option_timeout_description)] long timeout = 0,
            [Choice("sou", "s")]
            [Choice("pin", "p")]
            [Choice("man", "m")]
            [Choice(Resources.quizz_option_value_random, "r")]
            [Option(Resources.quizz_options_suit, Resources.quizz_options_suit_description)] string suit="r")
        {
            try
            {
                var generator = new ChinitsuQuizzGenerator(ctx.Client, suit);
                var nbProblems = Convert.ToInt32(nbRounds);
                var timeoutValue = Convert.ToInt32(timeout);
                var problem = new OngoingQuizz(
                    generator: generator,
                    timeout: timeoutValue,
                    nbQuestions: nbProblems
                );
                await kandoraContext.StartProblemSeries(ctx, problem, Resources.quizz_fullflush_startMessage, Resources.quizz_fullflush_threadName);
            }
            catch (Exception e)
            {
                replyWithException(ctx,e);
            }
        }

        [SlashCommand("nanikiru", Resources.quizz_nanikiru_description)]
        public async Task NanikiruProblem(InteractionContext ctx,
            [Option(Resources.quizz_options_questions, Resources.quizz_options_questions_description)] long nbRounds = 1,
            [Option(Resources.quizz_option_timeout, Resources.quizz_option_timeout_description)] long timeout = 0,
            [Option(Resources.quizz_options_uzaku, Resources.quizz_options_uzaku_description)] bool uzaku=true)
        {
            try
            {
                var generator = new NanikiruGenerator(ctx.Client, uzaku, timeout > 0); 
                var nbProblems = Convert.ToInt32(nbRounds);
                var timeoutValue = Convert.ToInt32(timeout);
                var problem = new OngoingNanikiru(
                    generator: generator,
                    timeout: timeoutValue,
                    nbQuestions: nbProblems,
                    client: ctx.Client
                );
                await kandoraContext.StartProblemSeries(ctx, problem, Resources.quizz_nanikiru_startMessage, Resources.quizz_nanikiru_threadName);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }
    }
}
