using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.exceptions;
using kandora.bot.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class ServerCommands : KandoraCommandModule
    {
        [Command("setAdmin"), Description("set user as server admin")]
        public async Task SetAdmin(CommandContext ctx, [Description("Target user")] DiscordUser user, [Description("YES or NO")] string flag)
        {
            if (flag.ToLower() != "yes" && flag.ToLower() != "no")
            {
                throw new Exception("wrong flag value. Required: YES / NO");
            }
            await executeCommand(
                ctx,
                getSetAdminAction(ctx, user, flag.ToLower() == "yes")
            );
        }
        private Func<Task> getSetAdminAction(CommandContext ctx, DiscordUser targetDiscordUser, bool flag)
        {
            return new Func<Task>(async () =>
            {
                var allUsers = UserDbService.GetUsers();
                var allServers = ServerDbService.GetServers(allUsers);
                var serverId = ctx.Guild.Id.ToString();
                var server = allServers[serverId];
                var userId = ctx.Member.Id.ToString();
                if (!server.Owners.Contains(allUsers[userId]))
                {
                    throw new Exception("You are not a owner of this server");
                }
                var targetId = targetDiscordUser.Id.ToString();

                ServerDbService.SetIsAdmin(serverId, targetId, flag);

            });
        }

        [Command("setOwner"), Description("set user as server owner")]
        public async Task SetOwner(CommandContext ctx, [Description("Target user")] DiscordUser user, [Description("YES or NO")] string flag)
        {
            if (flag.ToLower() != "yes" && flag.ToLower() != "no")
            {
                throw new Exception("wrong flag value. Required: YES / NO");
            }
            await executeCommand(
                ctx,
                getSetOwnerAction(ctx, user, flag.ToLower() == "yes")
            );
        }
        private Func<Task> getSetOwnerAction(CommandContext ctx, DiscordUser targetDiscordUser, bool flag)
        {
            return new Func<Task>(async () =>
            {
                var allUsers = UserDbService.GetUsers();
                var allServers = ServerDbService.GetServers(allUsers);
                var serverId = ctx.Guild.Id.ToString();
                var server = allServers[serverId];
                var userId = ctx.Member.Id.ToString();
                if (!server.Owners.Contains(allUsers[userId]))
                {
                    throw new Exception("You are not a owner of this bot");
                }
                var targetId = targetDiscordUser.Id.ToString();

                ServerDbService.SetIsOwner(serverId, targetId, flag);

            });
        }

        [Command("setTargetChannel"), Description("Set bot to listen to the current channel")]
        public async Task SetTargetChannel(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getSetTargetChannelAction(ctx),
                userMustBeInChannel: false,
                userMustBeRegistered: false
            );
        }

        private Func<Task> getSetTargetChannelAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                ServerDbService.SetTargetChannel(serverDiscordId, ctx.Channel.Id.ToString());
                await ctx.RespondAsync($"<#{ctx.Channel.Id}> has been registered as scoring channel");
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
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                UserDbService.CreateUser(discordId, serverDiscordId, config);
                ServerDbService.AddUserToServer(discordId, serverDiscordId, false, false);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId)) {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.GrantRoleAsync(ctx.Guild.Roles[roleId], "registering for riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
            });
        }

        [Command("registerdummy"), Description("Register dummy people")]
        public async Task RegisterDummy(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterDummyAction(ctx),
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterDummyAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                var heatiro = "323096688904634377";
                var clubapero = "198974501709414401";
                var Neral = "273192430172372993";
                UserDbService.CreateUser(heatiro, serverDiscordId, config);
                UserDbService.CreateUser(clubapero, serverDiscordId, config);
                UserDbService.CreateUser(Neral, serverDiscordId, config);
                ServerDbService.AddUserToServer(heatiro, serverDiscordId, false, false); //Heatiro
                ServerDbService.AddUserToServer(clubapero, serverDiscordId, false, false); //clubapero
                ServerDbService.AddUserToServer(Neral, serverDiscordId, false, false); //Neral
                UserDbService.SetMahjsoulName(heatiro, "heairo");
                UserDbService.SetMahjsoulName(Neral, "Neral");
                UserDbService.SetMahjsoulName(clubapero, "clubapero");
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