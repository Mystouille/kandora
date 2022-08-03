using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kandora.bot.services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.regular
{
    public class LeagueConfigCommands : KandoraCommandModule
    {
        [Command("displayLeagueConfig"), Description("Display the params for a specific league config")]
        public async Task DisplayLeagueConfig(CommandContext ctx,[Description("The name of the config")] string serverId=null)
        {
            await executeMpCommand(
                ctx,
                getDisplayLeagueConfigAction(ctx, serverId),
                userMustBeInMP: false
            );
        }

        private Func<Task> getDisplayLeagueConfigAction(CommandContext ctx, string inputServerId)
        {
            return new Func<Task>(async () =>
            {
                var serverId = inputServerId;
                if (serverId == null && ctx.Guild == null)
                {
                    return;
                }
                if( serverId == null)
                {
                    serverId = ctx.Guild.Id.ToString();
                }
                var userId = ctx.User.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                if (!allUsers.ContainsKey(userId))
                {
                    throw new Exception("You not registered in any server");
                }
                var allServers = ServerDbService.GetServers(allUsers);
                if (!allServers.ContainsKey(serverId))
                {
                    throw new Exception($"Server with id {serverId} does not exist");
                }
                var server = allServers[serverId];
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

                sb.AppendLine($"useEloSystem: {(config.UseEloSystem? "Yes" : "No")}");
                sb.AppendLine($"initialElo: {config.InitialElo}");
                sb.AppendLine($"minElo: {config.MinElo}");
                sb.AppendLine($"baseEloChangeDampening: {config.BaseEloChangeDampening}");
                sb.AppendLine($"eloChangeStartRatio: {config.EloChangeStartRatio}");
                sb.AppendLine($"eloChangeEndRatio: {config.EloChangeEndRatio}");
                sb.AppendLine($"trialPeriodDuration: {config.TrialPeriodDuration}");

                if (ctx.Member != null)
                {
                    await ctx.Member.SendMessageAsync(sb.ToString());
                }
                else
                {
                    await ctx.RespondAsync(sb.ToString());
                }
            });
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

        [Command("changeLeagueConfig"), Description("Change a param of a specific league config")]
        public async Task ChangeLeagueConfig(CommandContext ctx, [Description("The discord server id")] string serverId, [Description("What param to change")] string paramName, [Description("The value you want to set it to")] params string[] values)
        {
            await executeMpCommand(
                ctx,
                getChangeLeagueConfigAction(ctx, serverId, paramName, string.Join(' ',values)),
                userMustBeInMP: false
            );
        }

        private Func<Task> getChangeLeagueConfigAction(CommandContext ctx, string serverId, string paramName, string value)
        {
            return new Func<Task>(async () =>
            {
                CfgPrm property = CfgPrm.UNKNOWN;
                try
                {
                    property = (CfgPrm)Enum.Parse(typeof(CfgPrm), paramName);
                } catch {
                    throw new Exception("This is not an existing property");
                }
                var userId = ctx.User.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                var allServers = ServerDbService.GetServers(allUsers);
                if (!allServers.ContainsKey(serverId))
                {
                    throw new Exception($"Server with id {serverId} does not exist");
                }
                var server = allServers[serverId];
                if (!server.Admins.Select(x => x.Id).Contains(userId))
                {
                    throw new Exception("You must be admin of this server to change a league config");
                }
                var configId = server.LeagueConfigId;

                switch (property)
                {
                    case CfgPrm.name: LeagueConfigDbService.SetConfigValue(paramName, configId, value); break;
                    case CfgPrm.description: LeagueConfigDbService.SetConfigValue(paramName, configId, value); break;
                    case CfgPrm.countPoints: LeagueConfigDbService.SetConfigValue(paramName, configId, value.ToLower() == "yes"); break;
                    case CfgPrm.allowSanma: LeagueConfigDbService.SetConfigValue(paramName, configId, value.ToLower() == "yes"); break;
                    case CfgPrm.startingPoints: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma3p1: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma3p2: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma3p3: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma4p1: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma4p2: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma4p3: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.uma4p4: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.oka: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.penaltyLast: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.useEloSystem: LeagueConfigDbService.SetConfigValue(paramName, configId, value.ToLower() == "yes"); break;
                    case CfgPrm.initialElo: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.minElo: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.baseEloChangeDampening: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.eloChangeStartRatio: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.eloChangeEndRatio: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    case CfgPrm.trialPeriodDuration: LeagueConfigDbService.SetConfigValue(paramName, configId, float.Parse(value)); break;
                    default: throw new Exception($"You cannot change {paramName}");
                }
                await ctx.RespondAsync($"Changed param: {paramName}={value}");
            });
        }
    }
}