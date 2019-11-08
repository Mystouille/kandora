using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DescriptionAttribute = DSharpPlus.CommandsNext.Attributes.DescriptionAttribute;

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
                var isOk = UserDb.AddUser(displayName, discordId, mahjsoulId);
                await ctx.RespondAsync(isOk ?
                    $"<@{ctx.User.Id}> has been registered" :
                    $"Cancelled. <@{ctx.User.Id}> couldn't be registered");
            }
            catch(Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }


        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            try
            {
                var users = UserDb.GetUsers();
                int i = 1;

                foreach (var user in users)
                {
                    if (ctx != null && ctx.Member == null)
                    {
                        await ctx.RespondAsync($"{i}: <@{user.Id}> {user.MahjsoulId}");
                    }
                    else
                    {
                        await ctx.Member.SendMessageAsync($"{i}: <@{user.Id}> {user.MahjsoulId}");
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
        }
    }
}