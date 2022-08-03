using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using kandora.bot.resources;
using System;
using kandora.bot.services.discord.problems;

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
                await kandoraContext.StartProblemSeries(ctx, generator, Convert.ToInt32(nbRounds), Convert.ToInt32(timeout), Resources.quizz_fullflush_startMessage, Resources.quizz_fullflush_threadName);
            }
            catch (Exception e)
            {
                replyWithException(ctx,e);
            }
        }
    }
}
