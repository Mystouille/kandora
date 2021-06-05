using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.exceptions;
using kandora.bot.services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class UserCommands : KandoraCommandModule
    {
        [Command("setTargetChannel"), Description("Set bot to listen to the current channel")]
        public async Task SetTargetChannel(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getSetTargetChannelAction(ctx),
                mustBeInChannel: false,
                userMustBeRegistered: false
            );
        }

        private Func<Task> getSetTargetChannelAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                ServerDb.SetTargetChannel(serverDiscordId, ctx.Channel.Id.ToString());
                await ctx.RespondAsync($"<#{ctx.Channel.Id}> has been registered as scoring channel");
            });
        }

        [Command("activateLeague"), Description("register the current server as a league host")]
        public async Task RegisterServer(CommandContext ctx, [Description("A new user role name to ping people who are in the league")] string userRoleName)
        {
            await executeCommand(
                ctx,
                getRegisterServerAction(ctx, userRoleName),
                serverMustBeRegistered: false,
                mustBeInChannel: false,
                userMustBeRegistered: false
            );
        }
        private Func<Task> getRegisterServerAction(CommandContext ctx, string userRoleName)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var role = await ctx.Guild.CreateRoleAsync(name: userRoleName, mentionable: true);
                ServerDb.AddServer(serverDiscordId, ctx.Guild.Name, role.Id.ToString());
                await ctx.RespondAsync($"A Riichi league has started on {ctx.Guild.Name}!! \n The role {userRoleName} has also been created.");
            });
        }

        [Command("register"), Description("Register yourself in the kandora riichi league")]
        public async Task RegisterUser(CommandContext ctx, [Description("Your mahjsoul ID")] string mahjsoulId = "", [Description("Your tenhou ID")] string tenhouId = "")
        {
            await executeCommand(
                ctx,
                getRegisterUserAction(ctx, mahjsoulId, tenhouId),
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterUserAction(CommandContext ctx, string mahjsoulId, string tenhouId)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                if (!UserDb.IsUserInDb(discordId))
                {
                    UserDb.CreateUser(displayName, discordId, serverDiscordId, mahjsoulId, tenhouId);
                }
                var server = ServerDb.GetServer(serverDiscordId);
                ServerDb.AddUserToServer(discordId, serverDiscordId, false, false);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId)) {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.GrantRoleAsync(ctx.Guild.Roles[roleId], "registering for riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
            });
        }

        [Command("ping"), Description("test")]
        public async Task Ping(CommandContext ctx)
        {
            var bla = ctx.Member.Mention;
            var blu = $"<@{ctx.User.Id}>";
            await ctx.Channel.SendMessageAsync(bla);
            await ctx.Channel.SendMessageAsync(blu);
            await ctx.RespondAsync(bla);
            await ctx.RespondAsync(blu);
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
                var users = UserDb.GetUsers();
                var servers = ServerDb.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    throw new UserNotRegisteredException();
                }
                var server = servers[serverDiscordId];
                ServerDb.RemoveUserFromServer(discordId, serverDiscordId);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId))
                {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.RevokeRoleAsync(ctx.Guild.Roles[roleId], "removing from riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been removed from league");
            });
        }

        [Command("rename"), Description("Change your display name")]
        public async Task Rename(CommandContext ctx, [Description("Your new display name")] string displayName)
        {
            await executeCommand(
                ctx,
                getRenameAction(ctx, displayName)
            );
        }

        private Func<Task> getRenameAction(CommandContext ctx, string displayName)
        {
            return new Func<Task>(async () =>
            {
                var userId = ctx.User.Id.ToString();
                UserDb.SetDiplayName(userId, displayName);
                await ctx.RespondAsync($"<@{ctx.User.Id}>'s name has been changed to {displayName}.");
            });
        }

        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getListAction(ctx),
                userMustBeRegistered: false,
                mustBeInChannel: true
            );
        }

        private Func<Task> getListAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var users = UserDb.GetUsers();
                var servers = ServerDb.GetServers(users);
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();

                int i = 1;
                StringBuilder sb = new StringBuilder();
                sb.Append("User list:\n");
                foreach (var user in servers[serverDiscordId].Users)
                {
                    sb.Append($"{i}: <@{user.Id}> ({user.DisplayName}) mjslId:{user.MahjsoulId} {(user.Id == discordId ? " <<< you" : "")}\n");
                    i++;
                }
                if (ctx != null && ctx.Member == null)
                {
                    await ctx.RespondAsync(sb.ToString());
                }
                else
                {
                    await ctx.Member.SendMessageAsync(sb.ToString());
                }
            });
        }
    }
}