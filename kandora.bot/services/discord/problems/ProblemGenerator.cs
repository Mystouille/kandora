namespace kandora.bot.services.discord.problems
{
    public interface IQuizzGenerator {
        public MultipleChoicesQuestion GetNewQuestion();
    }
}