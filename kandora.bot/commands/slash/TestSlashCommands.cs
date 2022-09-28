using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using System;
using System.Linq;
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
                var roleName = "KandoraLeague";
                var roles = ctx.Guild.Roles.Where(x => x.Value.Name == roleName);
                if (roles.Count() > 0)
                {
                    await roles.First().Value.DeleteAsync();
                }

                //Add new
                var leagueConfigId = LeagueConfigDbService.CreateLeague();
                ulong roleId = ctx.Guild.Roles.Where(x => x.Value.Name == roleName).Select(x => x.Key).FirstOrDefault();
                if (roleId == 0)
                {
                    var role = await ctx.Guild.CreateRoleAsync(name: roleName, mentionable: true);
                    roleId = role.Id;
                }
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, roleId.ToString(), roleName, leagueConfigId);
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);

                var discordId = ctx.User.Id.ToString();
                var heatiro = "323096688904634377";
                var clubapero = "198974501709414401";
                var neral = "273192430172372993";
                var benoit = "159371527115112448";
                if (!users.ContainsKey(heatiro))
                {
                    UserDbService.CreateUser(heatiro, serverDiscordId, config);
                }
                if (!users.ContainsKey(clubapero))
                {
                    UserDbService.CreateUser(clubapero, serverDiscordId, config);
                }
                if (!users.ContainsKey(neral))
                {
                    UserDbService.CreateUser(neral, serverDiscordId, config);
                }
                if (!users.ContainsKey(benoit))
                {
                    UserDbService.CreateUser(benoit, serverDiscordId, config);
                }
                ServerDbService.AddUserToServer(heatiro, serverDiscordId, false); //Heatiro
                ServerDbService.AddUserToServer(clubapero, serverDiscordId, false); //clubapero
                ServerDbService.AddUserToServer(neral, serverDiscordId, false); //Neral
                UserDbService.SetMahjsoulName(heatiro, "heairo");
                UserDbService.SetMahjsoulName(neral, "Neral");
                UserDbService.SetMahjsoulName(clubapero, "clubapero");
                UserDbService.SetMahjsoulName(benoit, "Benoit");
                var playersStr = $"heairo: {heatiro}\nNeral: {neral}\nclubapero: {clubapero}\nBenoit: {benoit}";

                string[] players = { benoit, heatiro, clubapero, neral };
                float[] scores = { 50000, 4000, 20000, 10000 };
                ScoreDbService.RecordIRLGame(players, scores, server, config);
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_testLeague_testLeagueStarted, playersStr));
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }
    }
}
