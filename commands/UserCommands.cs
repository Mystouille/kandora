using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Kandora
{
    public class UserCommands
    {
        [Command("register"), Description("Register yourself in the kandora riichi league")]
        public async Task Register(CommandContext ctx, [Description("Your mahjsoul ID")] int mahjsoulId)
        {
            var displayName = ctx.User.Username;
            var discordId = ctx.User.Id;
            try
            {
                UserDb.AddUser(displayName, discordId, mahjsoulId);
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
                GlobalDb.Commit();
            }
            catch(Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
            }
        }

        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                var users = UserDb.GetUsers();

                int i = 1;
                StringBuilder sb = new StringBuilder();
                sb.Append("User list:\n");
                foreach (var user in users)
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
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
                GlobalDb.Rollback();
            }
        }
    }
}