using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kandora.bot.services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace kandora.bot.commands.regular
{
    public class InitCommands : KandoraCommandModule
    {
        [Command("activateLeague"), Description("register the current server as a league host and create the KandoraLeague role")]
        public async Task RegisterServer(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterServerAction(ctx),
                serverMustBeRegistered: false,
                userMustBeInChannel: false,
                userMustBeRegistered: false
            );
        }
        private Func<Task> getRegisterServerAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var leagueConfigId = LeagueConfigDbService.CreateLeague();
                var roleName = "KandoraLeague";
                ulong roleId = ctx.Guild.Roles.Where(x => x.Value.Name == roleName).Select(x => x.Key).FirstOrDefault();
                if (roleId == 0)
                {
                    var role = await ctx.Guild.CreateRoleAsync(name: roleName, mentionable: true);
                    roleId = role.Id;
                }
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, roleId.ToString(), roleName, leagueConfigId);
                await ctx.RespondAsync($"A Riichi league has started on {ctx.Guild.Name}!! \n");
            });
        }
        private Func<Task> getDeleteServerAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
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
                await ctx.RespondAsync($"Removed league and all its games from {ctx.Guild.Name}!! \n");
            });
        }

        [Command("register"), Description("Register yourself in the local riichi league")]
        public async Task RegisterUser(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterUserAction(ctx),
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterUserAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var discordId = ctx.User.Id.ToString();
                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverId];
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                if (!users.ContainsKey(discordId))
                {
                    UserDbService.CreateUser(discordId, serverId, config);
                }
                if (!server.Users.Select(x => x.Id).Contains(discordId)){
                    ServerDbService.AddUserToServer(discordId, serverId, false);
                }
                else
                {
                    throw new Exception("You are already registered in this server");
                }
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId)) {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.GrantRoleAsync(ctx.Guild.Roles[roleId], "registering for riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
            });
        }

        [Command("initTest"), Description("reboot league")]
        public async Task RegisterDummy(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterDummyAction(ctx),
                serverMustBeRegistered: false,
                userMustBeInChannel: false,
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterDummyAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                await getDeleteServerAction(ctx).Invoke();
                await getRegisterServerAction(ctx).Invoke();
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                var heatiro = "323096688904634377";
                var clubapero = "198974501709414401";
                var neral = "273192430172372993";
                var benoit = "159371527115112448";
                var users = UserDbService.GetUsers();
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
                await ctx.RespondAsync($"Added dummy users: {heatiro}, {clubapero}, {neral}, {benoit}");
            });
        }

        [Command("unregister"), Description("Remove yourself in the kandora riichi league")]
        public async Task UnRegisterUser(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getUnRegisterUserAction(ctx),
                userMustBeRegistered: true
            );
        }
        private Func<Task> getUnRegisterUserAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverDiscordId];
                ServerDbService.RemoveUserFromServer(discordId, serverDiscordId);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId))
                {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.RevokeRoleAsync(ctx.Guild.Roles[roleId], "removing from riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been removed from league");
            });
        }
    }
}