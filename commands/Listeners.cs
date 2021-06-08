using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using kandora.bot.utils;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class Listeners
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
                await OnPendingGameReaction(sender, msg, emoji, user, added);
            }
        }

        public async static Task OnPendingGameReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            var msgId = msg.Id;
            var userId = user.Id.ToString();
            var game = kanContext.PendingGames[msgId];
            bool result = false;
            var okEmoji = DiscordEmoji.FromName(sender, Reactions.OK);
            var noEmoji = DiscordEmoji.FromName(sender, Reactions.NO);
            if(emoji.Id == okEmoji.Id)
            {
                result = game.TryChangeUserOk(userId, isAdd: added);
            }
            else if (emoji.Id == noEmoji.Id)
            {
                result = game.TryChangeUserNo(userId, isAdd: added);
            }
            if (!result && sender.CurrentUser.Id != user.Id)
            {
                await msg.DeleteReactionAsync(emoji, user);
            }
        }
    }
}