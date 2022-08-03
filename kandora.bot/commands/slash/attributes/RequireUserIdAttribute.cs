using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash.attributes
{
    public class RequireUserIdAttribute : SlashCheckBaseAttribute
    {
        public ulong UserId;

        public RequireUserIdAttribute(ulong userId): base()
        {
            this.UserId = userId;
        }

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            if (ctx.User.Id == UserId)
                return true;
            else
                return false;
        }
    }
}
