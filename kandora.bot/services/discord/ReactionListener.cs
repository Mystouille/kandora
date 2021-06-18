using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class ReactionListener
    {
        private static KandoraContext kanContext = KandoraContext.Instance;
        public async static Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            await OnReaction(sender, e.Message, e.Emoji, e.User, added: true);
        }
        public async static Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            await OnReaction(sender, e.Message, e.Emoji, e.User, added:false);
        }

        public async static Task OnReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            if (kanContext.PendingGames.ContainsKey(msg.Id))
            {
                await PendingGame.OnPendingGameReaction(sender, msg, emoji, user, added);
            }
            else if (kanContext.OngoingProblems.ContainsKey(msg.Id))
            {
                var isOver = await kanContext.OngoingProblems[msg.Id].OnProblemReaction(sender, msg, emoji, user, added);

                if (isOver)
                {
                    kanContext.OngoingProblems.Remove(msg.Id);
                }
            }
        }
        
    }
}