﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.utils;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("admin", Resources.admin_groupDescription, defaultPermission: false)]
    class AdminSlashCommands : KandoraSlashCommandModule
    {
        [SlashCommand("startLeague", Resources.admin_startLeague_description)]
        public async Task StartLeague(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                if (servers.ContainsKey(serverDiscordId))
                {
                    throw new Exception(string.Format(Resources.admin_startLeague_leagueAlreadyExists, ctx.Guild.Name));
                }
                var leagueConfigId = LeagueConfigDbService.CreateLeague();
                var roleName = Resources.kandoraLeague_roleName;
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, "dummyRoleId", roleName, leagueConfigId);
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_startLeague_leagueStarted, ctx.Guild.Name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        //[SlashCommand("updateDisplayNames", "updateDisplayNames")]
        public async Task Updatenames(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var nbUpdated = 0;
                var discordUsers = await ctx.Guild.GetAllMembersAsync();
                foreach (var user in users){
                    if(user.Value.DisplayName != null && user.Value.DisplayName.Length > 0)
                    {
                        continue;
                    }
                    var discordUser = discordUsers.Where(x => x.Id.ToString() == user.Key).FirstOrDefault();
                    if(discordUser != null)
                    {
                        ServerDbService.SetUserDisplayName(serverDiscordId, user.Key, discordUser.DisplayName);
                        nbUpdated++;
                    }
                }
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format("Player display names updated: {0}", nbUpdated)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("endLeague", Resources.admin_endLeague_description)]
        public async Task EndLeague(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);

                if (!Bypass.isSuperUser(ctx.User.Id.ToString()))
                {
                    throw new Exception(Resources.admin_endLeague_unauthorized);
                }

                RankingDbService.DeleteRankings(serverDiscordId);
                ScoreDbService.DeleteGamesFromServer(serverDiscordId);
                ServerDbService.DeleteUsersFromServer(serverDiscordId);
                ServerDbService.DeleteServer(serverDiscordId);
                LeagueConfigDbService.DeleteLeagueConfig(server.LeagueConfigId);

                var rb = new DiscordInteractionResponseBuilder().WithContent(Resources.admin_endLeague_leagueEnded);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeConfig", Resources.admin_showLeagueConfig_description)]
        public async Task ShowLeagueConfig(InteractionContext ctx)
        {
            try
            {
                var configStr = GetLeagueConfig(ctx);
                var rb = new DiscordInteractionResponseBuilder().WithContent(configStr).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string GetLeagueConfig(InteractionContext ctx)
        {
            var serverDiscordId = ctx.Guild.Id.ToString();
            var server = ServerDbService.GetServer(serverDiscordId);
            var userId = ctx.User.Id.ToString();
            var configId = server.LeagueConfigId;
            var config = LeagueConfigDbService.GetLeagueConfig(configId);
            var sb = new StringBuilder();
            
            var mod = "**";
            sb.AppendLine($"{mod}startTime: {config.StartTime.ToString("yyyy/MM/dd")}{mod}");
            sb.AppendLine($"{mod}endTime: {(config.EndTime.ToString("yyyy/MM/dd"))}{mod}");
            sb.AppendLine($"{mod}countPoints: {(config.CountPoints ? "Yes" : "No")}{mod}");
            sb.AppendLine($"{mod}allowSanma: {(config.AllowSanma ? "Yes" : "No")}{mod}");
            sb.AppendLine($"{mod}EloSystem: {config.EloSystem}{mod}");
            mod = config.CountPoints ? "**" : "*";
            sb.AppendLine($"{mod}startingPoints: {config.StartingPoints}{mod}");
            mod = config.AllowSanma ? "**" : "*";
            sb.AppendLine($"{mod}uma3p1: {config.Uma3p1}{mod}");
            sb.AppendLine($"{mod}uma3p2: {config.Uma3p2}{mod}");
            sb.AppendLine($"{mod}uma3p3: {config.Uma3p3}{mod}");
            mod = "**";
            sb.AppendLine($"{mod}uma4p1: {config.Uma4p1}{mod}");
            sb.AppendLine($"{mod}uma4p2: {config.Uma4p2}{mod}");
            sb.AppendLine($"{mod}uma4p3: {config.Uma4p3}{mod}");
            sb.AppendLine($"{mod}uma4p4: {config.Uma4p4}{mod}");
            sb.AppendLine($"{mod}oka: {config.Oka}{mod}");
            sb.AppendLine($"{mod}penaltyLast: {config.PenaltyLast}{mod}");
            sb.AppendLine($"{mod}chomboPenalty: {config.PenaltyChombo}{mod}");
            mod = (config.EloSystem != "None" && config.EloSystem != "Average") ? "**" : "*";
            sb.AppendLine($"{mod}baseEloChangeDampening: {config.BaseEloChangeDampening}{mod}");
            mod = config.EloSystem == "Full" ? "**" : "*";
            sb.AppendLine($"{mod}initialElo: {config.InitialElo}{mod}");
            sb.AppendLine($"{mod}minElo: {config.MinElo}{mod}");
            sb.AppendLine($"{mod}eloChangeStartRatio: {config.EloChangeStartRatio}{mod}");
            sb.AppendLine($"{mod}eloChangeEndRatio: {config.EloChangeEndRatio}{mod}");
            sb.AppendLine($"{mod}trialPeriodDuration: {config.TrialPeriodDuration}{mod}");
            return sb.ToString();
        }

        [SlashCommand("setConfig", Resources.admin_setLeagueConfig_description)]
        public async Task SetLeagueConfig(InteractionContext ctx,
            [Option(Resources.admin_setLeagueConfig_countPoints, Resources.admin_setLeagueConfig_countPoints_description)] bool countPoints,
            [Choice(Resources.admin_setLeagueConfig_eloSystem_Average,"Average")]
            [Choice(Resources.admin_setLeagueConfig_eloSystem_None,"None")]
            [Choice(Resources.admin_setLeagueConfig_eloSystem_Simple,"Simple")]
            [Choice(Resources.admin_setLeagueConfig_eloSystem_Full,"Full")]
            [Option(Resources.admin_setLeagueConfig_eloSystem, Resources.admin_setLeagueConfig_eloSystem_description)] string eloSystem,
            [Option(Resources.admin_setLeagueConfig_startTime, Resources.admin_setLeagueConfig_startTime_description)] string startTime = "",
            [Option(Resources.admin_setLeagueConfig_endTime, Resources.admin_setLeagueConfig_endTime_description)] string endTime = "",
            [Option(Resources.admin_setLeagueConfig_startingPoints, Resources.admin_setLeagueConfig_startingPoints_description)] long startingPoints = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p1, Resources.admin_setLeagueConfig_uma3p1_description)] double uma3p1 = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p2, Resources.admin_setLeagueConfig_uma3p2_description)] double uma3p2 = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p3, Resources.admin_setLeagueConfig_uma3p3_description)] double uma3p3 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p1, Resources.admin_setLeagueConfig_uma4p1_description)] double uma4p1 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p2, Resources.admin_setLeagueConfig_uma4p2_description)] double uma4p2 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p3, Resources.admin_setLeagueConfig_uma4p3_description)] double uma4p3 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p4, Resources.admin_setLeagueConfig_uma4p4_description)] double uma4p4 = -1,
            [Option(Resources.admin_setLeagueConfig_oka, Resources.admin_setLeagueConfig_oka_description)] double oka = -1,
            [Option(Resources.admin_setLeagueConfig_penaltyLast, Resources.admin_setLeagueConfig_penaltyLast_description)] double penaltyLast = -1,
            [Option(Resources.admin_setLeagueConfig_penaltyChombo, Resources.admin_setLeagueConfig_penaltyChombo_description)] double penaltyChombo = -1,
            [Option(Resources.admin_setLeagueConfig_initialElo, Resources.admin_setLeagueConfig_initialElo_description)] long initialElo = -1,
            [Option(Resources.admin_setLeagueConfig_minElo, Resources.admin_setLeagueConfig_minElo_description)] long minElo = -1,
            [Option(Resources.admin_setLeagueConfig_eloChangeDampening, Resources.admin_setLeagueConfig_eloChangeDampening_description)] double eloChangeDampening = -1,
            [Option(Resources.admin_setLeagueConfig_eloChangeStartRatio, Resources.admin_setLeagueConfig_eloChangeStartRatio_description)] double eloChangeStartRatio = -1,
            [Option(Resources.admin_setLeagueConfig_eloChangeEndRatio, Resources.admin_setLeagueConfig_eloChangeEndRatio_description)] double eloChangeEndRatio = -1,
            [Option(Resources.admin_setLeagueConfig_trialPeriodDuration, Resources.admin_setLeagueConfig_trialPeriodDuration_description)] long trialPeriodDuration = -1)
        {
            var guid = Guid.NewGuid();
            var transactionName = $"setLeagueConfig/{guid}";
            DbService.Begin(transactionName);
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var userId = ctx.User.Id.ToString();
                var configId = server.LeagueConfigId;
                var config = LeagueConfigDbService.GetLeagueConfig(configId);

                if (startTime.Length > 0)
                {
                    var startDateTime = DateTime.ParseExact(startTime, "yyyy/MM/dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.startDateCol, configId, startDateTime);
                }
                if (endTime.Length > 0)
                {
                    var endDateTime = DateTime.ParseExact(endTime, "yyyy/MM/dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.endDateCol, configId, endDateTime);
                }
                //hardcoding this for now since it's gonna be pretty much useless until the bot becomes worldwide famous
                var allowSanma = false;
                if (allowSanma)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.allowSanmaCol, configId, true);
                    if (uma3p1 != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma3p1Col, configId, uma3p1);
                    }
                    if (uma3p2 != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma3p2Col, configId, uma3p2);
                    }
                    if (uma3p3 != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma3p3Col, configId, uma3p3);
                    }
                }

                if (uma4p1 != -1)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma4p1Col, configId, uma4p1);
                }
                if (uma4p2 != -1)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma4p2Col, configId, uma4p2);
                }
                if (uma4p3 != -1)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma4p3Col, configId, uma4p3);
                }
                if (uma4p3 != -1)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.uma4p4Col, configId, uma4p4);
                }

                if (countPoints)
                {
                    LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.countPointsCol, configId, true);
                    if( startingPoints != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.startingPointsCol, configId, startingPoints);
                    }

                    if (oka != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.okaCol, configId, oka);
                    }
                    if (penaltyLast != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.penaltyLastCol, configId, penaltyLast);
                    }
                    if (penaltyChombo != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.penaltyChomboCol, configId, penaltyChombo);
                    }
                    
                }
                LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.EloSystemCol, configId, eloSystem);

                if (eloSystem == "Full" || eloSystem == "Simple")
                {
                    if (initialElo != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.initialEloCol, configId, initialElo);
                    }
                    if (eloChangeDampening != -1)
                    {
                        LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.baseEloChangeDampeningCol, configId, eloChangeDampening);
                    }
                    if (eloSystem == "Full")
                    {
                        if (minElo != -1)
                        {
                            LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.minEloCol, configId, minElo);
                        }
                        if (eloChangeStartRatio != -1)
                        {
                            LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.eloChangeStartRatioCol, configId, eloChangeStartRatio);
                        }
                        if (eloChangeEndRatio != -1)
                        {
                            LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.eloChangeEndRatioCol, configId, eloChangeEndRatio);
                        }
                        if (trialPeriodDuration != -1)
                        {
                            LeagueConfigDbService.SetConfigValue(LeagueConfigDbService.trialPeriodDurationCol, configId, trialPeriodDuration);
                        }
                    }
                }
                var configStr = GetLeagueConfig(ctx);
                var rb = new DiscordInteractionResponseBuilder().WithContent(String.Format(Resources.admin_setLeagueConfig_backfillInProgress, configStr)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);

            }
            catch (Exception e)
            {
                DbService.Rollback(transactionName);
                replyWithException(ctx, e);
            }

            DbService.Commit(transactionName);

            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var userId = ctx.User.Id.ToString();
                var configId = server.LeagueConfigId;
                var config = LeagueConfigDbService.GetLeagueConfig(configId);
                var configStr = GetLeagueConfig(ctx);
                StringBuilder sb = new StringBuilder();
                var response = await ctx.GetOriginalResponseAsync();

                var wb = new DiscordWebhookBuilder().WithContent(String.Format(Resources.admin_setLeagueConfig_backfillFinished, configStr));
                RankingDbService.BackfillRankings(serverDiscordId, config);
                await ctx.EditResponseAsync(wb);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("addUser", Resources.admin_addPlayer_description)]
        public async Task AddGuestPlayer(InteractionContext ctx,
            [Option(Resources.admin_addPlayer_nickname, Resources.admin_addPlayer_nickname_description)] string name,
            [Option(Resources.leaderboard_register_mahjsoulName, Resources.leaderboard_register_mahjsoulName_description)] string mahjsoulName = "",
            [Option(Resources.leaderboard_register_tenhouName, Resources.leaderboard_register_tenhouName_description)] string tenhouName = ""
            )
        {
            try
            {
                var userId = getIdFromPlayerParam(name);
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverDiscordId];
                var configId = server.LeagueConfigId;
                var config = LeagueConfigDbService.GetLeagueConfig(configId);

                if (server.Users.Select(x => x.Id).Contains(userId))
                {
                    var nbGames = RankingDbService.GetUserRankingHistory(userId, server.Id).Where(x=>x.GameId>0 && x.OldRank!=null).Count();
                    throw (new Exception(String.Format(Resources.commandError_UserNicknameAlreadyExists,nbGames)));
                }

                if (!users.Keys.Contains(userId))
                {
                    UserDbService.CreateUser(userId, name, serverDiscordId, config);
                }
                RankingDbService.InitUserRanking(userId, serverDiscordId, config);
                

                if (mahjsoulName.Length == 0)
                {
                    UserDbService.SetMahjsoulName(userId, mahjsoulName);
                }
                if (tenhouName.Length == 0)
                {
                    UserDbService.SetTenhouName(userId, tenhouName);
                }

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_addPlayer_Success, name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string getIdFromPlayerParam(string playerStr)
        {
            if (playerStr.StartsWith("<@") && playerStr.EndsWith(">"))
            {
                return playerStr.Substring(2, playerStr.Length - 3);
            }
            return playerStr;
        }

        [SlashCommand("migrateUser", Resources.admin_migratePlayer_description)]
        public async Task MigratePlayer(InteractionContext ctx,
            [Option(Resources.admin_migratePlayer_sourceName, Resources.admin_migratePlayer_sourceName_description)] string sourceName,
            [Option(Resources.admin_migratePlayer_targetName, Resources.admin_migratePlayer_targetName_description)] string targetName
            )
        {
            var transactionName = $"migration:{sourceName}-{targetName}";
            DbService.Begin(transactionName);
            try
            {

                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverId];
                var configId = server.LeagueConfigId;
                var config = LeagueConfigDbService.GetLeagueConfig(configId);
                var userId = getIdFromPlayerParam(sourceName);
                var targetuserId = getIdFromPlayerParam(targetName);


                //Check if user exists
                if (!server.Users.Select(x => x.Id).Contains(userId))
                {
                    throw (new Exception(Resources.commandError_PlayerUnknown));
                }
                //Create new user in server/db if he's not here already
                if (!users.Keys.Contains(targetuserId))
                {
                    UserDbService.CreateUser(targetuserId, targetName, serverId, config);
                }
                else if (!server.Users.Select(x => x.Id).Contains(targetuserId))
                {
                    ServerDbService.AddUserToServer(targetuserId, serverId, targetName);
                }

                //Change rankings reference to user
                RankingDbService.ChangeUserNameInRankings(userId, targetuserId, serverId);

                //Change games reference to user
                ScoreDbService.ChangeUserNameInGames(userId, targetuserId, serverId);

                //Remove old user from server
                ServerDbService.RemoveUserFromServer(userId,serverId);

                //Update the rankings
                RankingDbService.BackfillRankings(serverId, config);


                DbService.Commit(transactionName);

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_migratePlayer_success, sourceName, targetuserId)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                DbService.Rollback(transactionName);
                replyWithException(ctx, e);
            }
        }
        enum CfgPrm
        {
            server,
            name,
            description,
            allowSanma,
            countPoints,
            startingPoints,
            uma3p1,
            uma3p2,
            uma3p3,
            uma4p1,
            uma4p2,
            uma4p3,
            uma4p4,
            oka,
            penaltyLast,
            useEloSystem,
            initialElo,
            minElo,
            baseEloChangeDampening,
            eloChangeStartRatio,
            eloChangeEndRatio,
            trialPeriodDuration,
            UNKNOWN
        }
    }
}
