using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.utils;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("test", Resources.admin_groupDescription, defaultPermission: false)]
    class TestSlashCommands : KandoraSlashCommandModule
    {
        [SlashCommand("testLeague", Resources.admin_testLeague_description)]
        public async Task TestLeague(InteractionContext ctx)
        {
            try
            {
                //Delete existing:
                var serverDiscordId = ctx.Guild.Id.ToString();
                RankingDbService.DeleteRankings(serverDiscordId);
                ScoreDbService.DeleteGamesFromServer(serverDiscordId);
                ServerDbService.DeleteUsersFromServer(serverDiscordId);
                ServerDbService.DeleteServer(serverDiscordId);
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                if (servers.ContainsKey(serverDiscordId))
                {
                    LeagueConfigDbService.DeleteLeagueConfig(servers[serverDiscordId].LeagueConfigId);
                }

                //Add new
                var leagueConfigId = LeagueConfigDbService.CreateLeague();
                
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, "unknownRoleId", "unknownRole", leagueConfigId);
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);

                var discordId = ctx.User.Id.ToString();
                var arrcival = "88011881498812416";
                var dustray = "268421799526531073";
                var ivy = "473916716796084238";
                var benoit = "159371527115112448";
                if (!users.ContainsKey(arrcival))
                {
                    UserDbService.CreateUser(arrcival, serverDiscordId, config);
                    RankingDbService.InitUserRanking(arrcival, serverDiscordId, config);
                }
                if (!users.ContainsKey(dustray))
                {
                    UserDbService.CreateUser(dustray, serverDiscordId, config);
                    RankingDbService.InitUserRanking(dustray, serverDiscordId, config);
                }
                if (!users.ContainsKey(ivy))
                {
                    UserDbService.CreateUser(ivy, serverDiscordId, config);
                    RankingDbService.InitUserRanking(ivy, serverDiscordId, config);
                }
                if (!users.ContainsKey(benoit))
                {
                    UserDbService.CreateUser(benoit, serverDiscordId, config);
                    RankingDbService.InitUserRanking(benoit, serverDiscordId, config);
                }
                ServerDbService.AddUserToServer(arrcival, serverDiscordId, false); //Heatiro
                ServerDbService.AddUserToServer(dustray, serverDiscordId, false); //clubapero
                ServerDbService.AddUserToServer(ivy, serverDiscordId, false); //Neral
                ServerDbService.AddUserToServer(benoit, serverDiscordId, false); //Neral
                UserDbService.SetMahjsoulName(arrcival, "Arrcival");
                UserDbService.SetMahjsoulName(dustray, "Dustray");
                UserDbService.SetMahjsoulName(ivy, "Ivyyy");
                UserDbService.SetMahjsoulName(benoit, "Benoit");
                var playersStr = $"arrcival dustray ivy benoit";

                string[] players = { benoit, dustray, ivy, arrcival };
                int[] scores = { 50000, 4000, 20000, 10000 };
                int[] chombos = { 0, 0, 0, 0 };
                string location = "nowhere";
                ScoreDbService.RecordIRLGame(players, scores, chombos, DateTime.Now, location, server, config);
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_testLeague_testLeagueStarted, playersStr));
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("testConfig", "test config")]
        public async Task TestConfig(InteractionContext ctx)
        {
            try
            {
                var settings = ConfigurationManager.AppSettings;
                var countPoints = bool.Parse(settings.Get("countPoints"));
                var useEloSystem = bool.Parse(settings.Get("useEloSystem"));
                var startingPoints = Double.Parse(settings.Get("startingPoints"));
                var allowSanma = Boolean.Parse(settings.Get("allowSanma"));
                var uma3p1 = Double.Parse(settings.Get("uma3p1"));
                var uma3p2 = Double.Parse(settings.Get("uma3p2"));
                var uma3p3 = Double.Parse(settings.Get("uma3p3"));
                var uma4p1 = Double.Parse(settings.Get("uma4p1"));
                var uma4p2 = Double.Parse(settings.Get("uma4p2"));
                var uma4p3 = Double.Parse(settings.Get("uma4p3"));
                var uma4p4 = Double.Parse(settings.Get("uma4p4"));
                var oka = Double.Parse(settings.Get("oka"));
                var penaltyLast = Double.Parse(settings.Get("penaltyLast"));
                var initialElo = Double.Parse(settings.Get("initialElo"));
                var minElo = Double.Parse(settings.Get("minElo"));
                var baseEloChangeDampening = Double.Parse(settings.Get("baseEloChangeDampening"));
                var eloChangeStartRatio = Double.Parse(settings.Get("eloChangeStartRatio"));
                var eloChangeEndRatio = Double.Parse(settings.Get("eloChangeEndRatio"));
                var trialPeriodDuration = Double.Parse(settings.Get("trialPeriodDuration"));

                var sb = new StringBuilder();
                sb.AppendLine($"countPoints: {countPoints}");
                sb.AppendLine($"useEloSystem: {useEloSystem}");
                sb.AppendLine($"startingPoints: {startingPoints}");
                sb.AppendLine($"allowSanma: {allowSanma}");
                sb.AppendLine($"uma3p1: {uma3p1}");
                sb.AppendLine($"uma3p2: {uma3p2}");
                sb.AppendLine($"uma3p3: {uma3p3}");
                sb.AppendLine($"uma4p1: {uma4p1}");
                sb.AppendLine($"uma4p2: {uma4p2}");
                sb.AppendLine($"uma4p3: {uma4p3}");
                sb.AppendLine($"uma4p4: {uma4p4}");
                sb.AppendLine($"oka: {oka}");
                sb.AppendLine($"penaltyLast: {penaltyLast}");
                sb.AppendLine($"initialElo: {initialElo}");
                sb.AppendLine($"minElo: {minElo}");
                sb.AppendLine($"baseEloChangeDampening: {baseEloChangeDampening}");
                sb.AppendLine($"eloChangeStartRatio: {eloChangeStartRatio}");
                sb.AppendLine($"eloChangeEndRatio: {eloChangeEndRatio}");
                sb.AppendLine($"trialPeriodDuration: {trialPeriodDuration}");
                var rb = new DiscordInteractionResponseBuilder().WithContent(sb.ToString()).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }
    }
}
