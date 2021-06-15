using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using kandora.bot.utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public sealed class KandoraContext
    {
         private static readonly KandoraContext instance = new KandoraContext();


        static KandoraContext()
        {
        }

        private KandoraContext()
        {
            PendingGames = new Dictionary<ulong, PendingGame>();
            OngoingProblems = new Dictionary<ulong, OngoingProblem>();
        }
        public static KandoraContext Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<ulong, PendingGame> PendingGames { get; }
        public Dictionary<ulong, OngoingProblem> OngoingProblems { get; }

        public async Task AddPendingGame(CommandContext ctx, DiscordMessage msg, PendingGame game)
        {
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.OK));
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.NO));
            PendingGames.Add(msg.Id, game);
        }
        public async Task AddOngoingProblem(CommandContext ctx, DiscordMessage msg, ISet<int> answer, string options)
        {
            var optionsEmoji = HandParser.GetHandEmojiCodes(options, ctx.Client);
            var problem = new OngoingProblem(answer);
            foreach (var emoji in optionsEmoji)
            {
                await msg.CreateReactionAsync(emoji);
                problem.Options.Add(emoji.Id);
            }
            OngoingProblems.Add(msg.Id, problem);
        }

    }
}
