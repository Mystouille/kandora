using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using kandora.bot.models;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
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
        }
        public static KandoraContext Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<ulong, PendingGame> PendingGames { get; }

        public async Task AddPendingGame(CommandContext ctx, DiscordMessage msg, PendingGame game)
        {
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client,Reactions.OK));
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, Reactions.NO));
            PendingGames.Add(msg.Id, game);
        }

    }
}
