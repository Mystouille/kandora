﻿using DSharpPlus;
using DSharpPlus.Entities;
using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services.db;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kandora.bot.services.discord
{
    public class PendingGame
    {
        public PendingGame(string[] userIds, Server server, RiichiGame log)
        {
            UserIds = userIds;
            Log = log;
            Server = server;
            usersOk = new HashSet<string>();
            usersNo = new HashSet<string>();
        }
        public PendingGame(string[] userIds, int[] scores, int[] chombos, string location, DateTime timestamp, Server server)
        {
            UserIds = userIds;
            Scores = scores;
            Server = server;
            usersOk = new HashSet<string>();
            usersNo = new HashSet<string>();
            TimeStamp = timestamp;
            Chombos = chombos;
            Location = location;
        }

        private ISet<string> usersOk;
        private ISet<string> usersNo;
        public string[] UserIds { get; }
        public int[] Scores { get; }
        public int[] Chombos { get; }
        public string Location { get; }
        public RiichiGame Log { get; }
        public Server Server { get; }
        public DateTime TimeStamp { get; }
        public bool TryChangeUserOk(string userId, bool isAdd)
        {
            return TryChangeSet(usersOk, userId, isAdd);
        }
        public bool TryChangeUserNo(string userId, bool isAdd)
        {
            return TryChangeSet(usersNo, userId, isAdd);
        }
        public bool IsCancelled
        {
            get {
                return usersNo.Count == 2 || usersNo.Where(x => Bypass.isSuperUser(x)).Any();
            }
        }
        public bool IsValidated
        {
            get
            {
                return usersOk.Count == 2 || usersOk.Where(x=>Bypass.isSuperUser(x)).Any();
            }
        }
        private bool TryChangeSet(ISet<string> set, string userId, bool isAdd)
        {
            if (UserIds.Contains(userId) || Bypass.isKandora(userId) || Bypass.isSuperUser(userId))
            {
                if (isAdd)
                {
                    set.Add(userId);
                }
                else
                {
                    set.Remove(userId);
                }
                return true;
            }
            return false;
        }

        public async static Task OnPendingGameReaction(DiscordClient sender, DiscordMessage msg, DiscordEmoji emoji, DiscordUser user, bool added)
        {
            var kanContext = KandoraSlashContext.Instance;
            var msgId = msg.Id;
            var userId = user.Id.ToString();
            var game = kanContext.PendingGames[msgId];
            bool result = false;
            var okEmoji = DiscordEmoji.FromName(sender, Reactions.OK);
            var noEmoji = DiscordEmoji.FromName(sender, Reactions.NO);
            if (emoji.Name == okEmoji.Name)
            {
                result = game.TryChangeUserOk(userId, isAdd: added);
            }
            else if (emoji.Name == noEmoji.Name)
            {
                result = game.TryChangeUserNo(userId, isAdd: added);
            }
            if (!result && added)
            {
                await msg.DeleteReactionAsync(emoji, user);
            }
            if (game.IsCancelled)
            {
                await msg.ModifyAsync(String.Format(Resources.leaderboard_submitResult_canceledMessage,noEmoji));
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
                    if (server.LeaderboardConfigId == null)
                    {
                        throw new Exception(Resources.commandError_leaderboardNotInitialized);
                    }
                    var leaderboardConfig = ConfigDbService.GetConfig((int)(server.LeaderboardConfigId));


                    if (game.Log == null)
                    {
                        ScoreDbService.RecordIRLGame(game.UserIds, game.Scores, game.Chombos, game.TimeStamp, game.Location, server, leaderboardConfig);
                    }
                    else
                    {
                        ScoreDbService.RecordOnlineGame(game.Log, server, leaderboardConfig);
                    }
                    kanContext.PendingGames.Remove(msgId);

                    DbService.Commit("recordgame");
                    await msg.ModifyAsync($"{msg.Content.ToString()}\n{String.Format(Resources.leaderboard_submitResult_validatedMessage, okEmoji)}");
                    await msg.DeleteReactionsEmojiAsync(okEmoji);
                    await msg.DeleteReactionsEmojiAsync(noEmoji);
                }
                catch (Exception e)
                {
                    DbService.Rollback("recordgame");
                    await msg.RespondAsync(e.Message + "\n" + e.StackTrace);
                }
            }
        }
    }
}
