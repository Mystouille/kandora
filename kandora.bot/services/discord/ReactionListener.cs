using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class ReactionListener
    {
        private static KandoraSlashContext kanContext = KandoraSlashContext.Instance;
        public async static Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            await kanContext.NotifyReaction(sender, e.Message, e.Emoji, e.User, added: true).ConfigureAwait(true);
        }
        public async static Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            await kanContext.NotifyReaction(sender, e.Message, e.Emoji, e.User, added:false).ConfigureAwait(true);
        }        
    }
}