using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.db;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ulong roleId = ctx.Guild.Roles.Where(x => x.Value.Name == roleName).Select(x => x.Key).FirstOrDefault();
                if (roleId == 0)
                {
                    var role = await ctx.Guild.CreateRoleAsync(name: roleName, mentionable: true);
                    roleId = role.Id;
                }
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, roleId.ToString(), roleName, leagueConfigId);
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_startLeague_leagueStarted, ctx.Guild.Name));
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

                RankingDbService.DeleteRankings(serverDiscordId);
                ScoreDbService.DeleteGamesFromServer(serverDiscordId);
                ServerDbService.DeleteUsersFromServer(serverDiscordId);
                ServerDbService.DeleteServer(serverDiscordId);
                LeagueConfigDbService.DeleteLeagueConfig(server.LeagueConfigId);

                var roleName = "KandoraLeague";
                var roles = ctx.Guild.Roles.Where(x => x.Value.Name == roleName);
                if (roles.Count() > 0)
                {
                    await roles.First().Value.DeleteAsync();
                }

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
            sb.AppendLine($"countPoints: {(config.CountPoints ? "Yes" : "No")}");
            sb.AppendLine($"startingPoints: {config.StartingPoints}");
            sb.AppendLine($"allowSanma: {(config.AllowSanma ? "Yes" : "No")}");
            sb.AppendLine($"startingPoints: {config.StartingPoints}");
            sb.AppendLine($"uma3p1: {config.Uma3p1}");
            sb.AppendLine($"uma3p2: {config.Uma3p2}");
            sb.AppendLine($"uma3p3: {config.Uma3p3}");
            sb.AppendLine($"uma4p1: {config.Uma4p1}");
            sb.AppendLine($"uma4p2: {config.Uma4p2}");
            sb.AppendLine($"uma4p3: {config.Uma4p3}");
            sb.AppendLine($"uma4p4: {config.Uma4p4}");
            sb.AppendLine($"oka: {config.Oka}");
            sb.AppendLine($"penaltyLast: {config.PenaltyLast}");

            sb.AppendLine($"useEloSystem: {(config.UseEloSystem ? "Yes" : "No")}");
            sb.AppendLine($"initialElo: {config.InitialElo}");
            sb.AppendLine($"minElo: {config.MinElo}");
            sb.AppendLine($"baseEloChangeDampening: {config.BaseEloChangeDampening}");
            sb.AppendLine($"eloChangeStartRatio: {config.EloChangeStartRatio}");
            sb.AppendLine($"eloChangeEndRatio: {config.EloChangeEndRatio}");
            sb.AppendLine($"trialPeriodDuration: {config.TrialPeriodDuration}");
            return sb.ToString();
        }

        [SlashCommand("setConfig", Resources.admin_setLeagueConfig_description)]
        public async Task SetLeagueConfig(InteractionContext ctx,
            [Option(Resources.admin_setLeagueConfig_allowSanma, Resources.admin_setLeagueConfig_allowSanma_description)] bool allowSanma,
            [Option(Resources.admin_setLeagueConfig_countPoints, Resources.admin_setLeagueConfig_countPoints_description)] bool countPoints,
            [Option(Resources.admin_setLeagueConfig_useEloSystem, Resources.admin_setLeagueConfig_useEloSystem_description)] bool useEloSystem,
            [Option(Resources.admin_setLeagueConfig_startingPoints, Resources.admin_setLeagueConfig_startingPoints_description)] long startingPoints = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p1, Resources.admin_setLeagueConfig_uma3p1_description)] long uma3p1 = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p2, Resources.admin_setLeagueConfig_uma3p2_description)] long uma3p2 = -1,
            [Option(Resources.admin_setLeagueConfig_uma3p3, Resources.admin_setLeagueConfig_uma3p3_description)] long uma3p3 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p1, Resources.admin_setLeagueConfig_uma4p1_description)] long uma4p1 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p2, Resources.admin_setLeagueConfig_uma4p2_description)] long uma4p2 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p3, Resources.admin_setLeagueConfig_uma4p3_description)] long uma4p3 = -1,
            [Option(Resources.admin_setLeagueConfig_uma4p4, Resources.admin_setLeagueConfig_uma4p4_description)] long uma4p4 = -1,
            [Option(Resources.admin_setLeagueConfig_oka, Resources.admin_setLeagueConfig_oka_description)] long oka = -1,
            [Option(Resources.admin_setLeagueConfig_penaltyLast, Resources.admin_setLeagueConfig_penaltyLast_description)] long penaltyLast = -1,
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

                if (allowSanma)
                {
                    LeagueConfigDbService.SetConfigValue("allowSanma", configId, true);
                    if (uma3p1 >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("uma3p1", configId, uma3p1);
                    }
                    if (uma3p2 >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("uma3p2", configId, uma3p2);
                    }
                    if (uma3p3 >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("uma3p3", configId, uma3p3);
                    }
                }

                if (uma4p1 >= 0)
                {
                    LeagueConfigDbService.SetConfigValue("uma4p1", configId, uma4p1);
                }
                if (uma4p2 >= 0)
                {
                    LeagueConfigDbService.SetConfigValue("uma4p2", configId, uma4p2);
                }
                if (uma4p3 >= 0)
                {
                    LeagueConfigDbService.SetConfigValue("uma4p3", configId, uma4p3);
                }
                if (uma4p3 >= 0)
                {
                    LeagueConfigDbService.SetConfigValue("uma4p4", configId, uma4p4);
                }

                if (countPoints)
                {
                    LeagueConfigDbService.SetConfigValue("countPoints", configId, true);
                    if( startingPoints >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("startingPoints", configId, startingPoints);
                    }

                    if (oka >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("oka", configId, oka);
                    }
                    if (penaltyLast >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("penaltyLast", configId, penaltyLast);
                    }
                }
                if (useEloSystem)
                {
                    LeagueConfigDbService.SetConfigValue("useEloSystem", configId, useEloSystem);
                    if (initialElo >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("initialElo", configId, initialElo);
                    }
                    if (minElo >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("minElo", configId, minElo);
                    }
                    if (eloChangeDampening >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("baseEloChangeDampening", configId, eloChangeDampening);
                    }
                    if (eloChangeStartRatio >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("eloChangeStartRatio", configId, eloChangeStartRatio);
                    }
                    if (eloChangeEndRatio >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("eloChangeEndRatio", configId, eloChangeEndRatio);
                    }
                    if (trialPeriodDuration >= 0)
                    {
                        LeagueConfigDbService.SetConfigValue("trialPeriodDuration", configId, trialPeriodDuration);
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
                RankingDbService.BackfillRankings(server, config);
                await ctx.EditResponseAsync(wb);
            }
            catch (Exception e)
            {
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
