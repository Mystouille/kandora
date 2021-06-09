using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.utils;
using System;
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
            if (!result && added)
            {
                await msg.DeleteReactionAsync(emoji, user);
            }
            if (game.IsCancelled)
            {
                await msg.ModifyAsync($"All players have voted {noEmoji}, this log won't be recorded");
                kanContext.PendingGames.Remove(msgId);
            }
            if (game.IsValidated)
            {

                DbService.Begin("recordgame");
                try
                {
                    var serverId = msg.Channel.GuildId.ToString();
                    var users = UserDbService.GetUsers();
                    var servers = ServerDbService.GetServers(users);
                    var server = servers[serverId];
                    var leagueConfig = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);

                    if (game.Log == null)
                    {
                        ScoreDbService.RecordIRLGame(game.UserIds, game.Scores, server, leagueConfig);
                    }
                    else
                    {
                        ScoreDbService.RecordOnlineGame(game.Log, server);
                    }
                    kanContext.PendingGames.Remove(msgId);
                    await msg.RespondAsync($"All players have voted {okEmoji}, this log has been recorded!");
                    await msg.DeleteReactionsEmojiAsync(okEmoji);
                    await msg.DeleteReactionsEmojiAsync(noEmoji);

                    DbService.Commit("recordgame");
                }
                catch (Exception e)
                {
                    DbService.Rollback("recordgame");
                    await msg.RespondAsync(e.Message);
                }
            }
        }
    }
}