using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Kandora
{
    public class UserCommands : BaseCommandModule
    {
        [Command("register"), Description("Register yourself in the kandora riichi league")]
        public async Task Register(CommandContext ctx, [Description("Your mahjsoul ID")] string mahjsoulId, [Description("Your tenhou ID")] string tenhouId)
        {
            var displayName = ctx.User.Username;
            var discordId = ctx.User.Id.ToString();
            try
            {
                GlobalDb.Begin("register");
                UserDb.AddUser(displayName, discordId, mahjsoulId, tenhouId);
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
                GlobalDb.Commit("register");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("register");
            }
        }

        [Command("rename"), Description("Change your display name")]
        public async Task Rename(CommandContext ctx, [Description("Your new display name")] string displayName)
        {
            var discordId = ctx.User.Id.ToString();
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                GlobalDb.Begin("rename");
                var userList = UserDb.GetUsers();
                var serverList = ServerDb.GetServers(userList);
                if (!userList.ContainsKey(discordId))
                {
                    throw new Exception($"<@{ctx.User.Id}> is not registered yet, use !register to enter the league.");
                }
                var user = userList[discordId];
                var oldName = user.DisplayName;
                user.DisplayName = displayName;
                await ctx.RespondAsync($"<@{ctx.User.Id}>'s name has been changed from {oldName} to {displayName}.");
                GlobalDb.Commit("rename");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("rename");
            }
        }

        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                GlobalDb.Begin("listusers");
                var users = UserDb.GetUsers();

                int i = 1;
                StringBuilder sb = new StringBuilder();
                sb.Append("User list:\n");
                foreach (var user in users.Values)
                {
                    sb.Append($"{i}: <@{user.Id}> ({user.DisplayName}) {user.MahjsoulId}\n");
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
                GlobalDb.Commit("listusers");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback("listusers");
            }
        }
    }
}