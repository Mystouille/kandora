using DSharpPlus;
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
            OngoingProblems = new Dictionary<ulong, OngoingQuizz>();
            OngoingTimedProblems = new Dictionary<ulong, OngoingQuizz>();
        }
        public static KandoraContext Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<ulong, PendingGame> PendingGames { get; }
        public Dictionary<ulong, OngoingQuizz> OngoingProblems { get; }
        public Dictionary<ulong, OngoingQuizz> OngoingTimedProblems { get; }

        public async Task AddPendingGame(CommandContext ctx, DiscordMessage msg, PendingGame game)
        {
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.OK)).ContinueWith(x =>
            {
                msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.NO));
            }).ContinueWith(x =>
            {
                PendingGames.Add(msg.Id, game);
            });
        }

        public async Task AddOngoingProblem(DiscordClient client, DiscordMessage msg, ISet<int> answer, string options, int timeout = 0)
        {
            /**
            var optionsEmoji = HandParser.GetHandEmojiCodes(options, client);
            var problem = new OngoingProblem(answer, hasTimer: timeout>0);
            foreach (var emoji in optionsEmoji)
            {
                await msg.CreateReactionAsync(emoji);
                problem.Options.Add(emoji.Id);
            }
            OngoingProblems.Add(msg.Id, problem);

            if (timeout > 0)
            {
                await Task.Delay(10000).ContinueWith(parent => {
                    _ = problem.OnProblemTimeout(client, msg);
                    OngoingProblems.Remove(msg.Id);
                });
            }
            */

        }


    }
}
