﻿using DSharpPlus.CommandsNext;
using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.services.discord;
using System;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class KandoraCommandModule: BaseCommandModule
    {
        protected static KandoraContext context = KandoraContext.Instance;

        protected static async Task executeCommand(CommandContext ctx, Func<Task> command, bool userMustBeRegistered = true, bool serverMustBeRegistered = true)
        {
            var commandStr = "command";
            try
            {
                string serverId = (ctx.Guild.Id.ToString());
                string channelId = (ctx.Channel.Id.ToString());
                string userId = (ctx.User.Id.ToString());
                Server server = null;
                if (serverMustBeRegistered)
                {
                    server = ServerDbService.GetServer(serverId);
                }
                if (userMustBeRegistered && !ServerDbService.isUserOnServer(userId, serverId))
                {
                    throw new Exception($"<@{userId}> is not registered yet, use !register to enter the league.");
                }
                DbService.Begin(commandStr);
                await command.Invoke();
                DbService.Commit(commandStr);
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    await ctx.RespondAsync(e.Message);
                }
                DbService.Rollback(commandStr);
            }
        }

        protected static async Task executeMpCommand(CommandContext ctx, Func<Task> command, bool userMustBeInMP = true)
        {
            var commandStr = "command";
            try
            {
                if (userMustBeInMP && ctx.Member!= null)
                {
                    throw new SilentException();
                }
                DbService.Begin(commandStr);
                await command.Invoke();
                DbService.Commit(commandStr);
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    if(ctx.Member != null)
                    {
                        await ctx.Member.SendMessageAsync(e.Message);
                    }
                    else
                    {
                        await ctx.RespondAsync(e.Message);
                    }
                }
                DbService.Rollback(commandStr);
            }
        }
    }
}
