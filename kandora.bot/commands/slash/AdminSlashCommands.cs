using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
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

        [SlashCommand("showLeagueConfig", Resources.admin_endLeague_description)]
        public async Task ShowLeagueConfig(InteractionContext ctx)
        {
            try
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
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }
    }
}
