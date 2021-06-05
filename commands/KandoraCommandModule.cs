using DSharpPlus.CommandsNext;
using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services;
using System;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class KandoraCommandModule: BaseCommandModule
    {
        protected static async Task executeCommand(CommandContext ctx, Func<Task> command, bool userMustBeRegistered = true, bool mustBeInChannel = true, bool serverMustBeRegistered = true)
        {
            var commandStr = "command";
            try
            {
                if (mustBeInChannel && (ctx.Channel == null || ctx.Guild == null))
                {
                    throw (new NotInChannelException());
                }
                string serverId = (ctx.Guild.Id.ToString());
                string channelId = (ctx.Channel.Id.ToString());
                string userId = (ctx.User.Id.ToString());
                Server server = null;
                if (serverMustBeRegistered || mustBeInChannel)
                {
                    server = ServerDb.GetServer(serverId);
                }
                if (mustBeInChannel && (server == null || server.TargetChannelId != channelId))
                {
                    throw new SilentException();
                }
                if (userMustBeRegistered && !ServerDb.isUserOnServer(userId, serverId))
                {
                    throw new Exception($"<@{userId}> is not registered yet, use !register to enter the league.");
                }
                GlobalDb.Begin(commandStr);
                await command.Invoke();
                GlobalDb.Commit(commandStr);
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    await ctx.RespondAsync(e.Message);
                }
                GlobalDb.Rollback(commandStr);
            }
        }
    }
}
