using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using kandora.bot.resources;
using kandora.bot.services;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash.attributes
{
    public class KandoraRequirePermissionsAttribute: SlashCheckBaseAttribute
    {
        private bool ForKandoraAdmin { get; }
        private SlashRequirePermissionsAttribute SlashAttribute { get; }
        public KandoraRequirePermissionsAttribute(Permissions permissions, bool forKandoraAdmin = false, bool ignoreDms = true)
        {
            SlashAttribute = new SlashRequirePermissionsAttribute(permissions, ignoreDms);
            this.ForKandoraAdmin = forKandoraAdmin;
        }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            return SlashAttribute.ExecuteChecksAsync(ctx).ContinueWith(result => checkResult(result.Result, ctx));
            ;
        }

        private bool checkResult(bool result, InteractionContext ctx)
        {
            if (!result)
            {
                ReplyWithError(ctx, "can't execute that!");
            }
            return result;
        }
        private async void ReplyWithError(InteractionContext ctx, string message)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = Resources.commandAttributeError_title,
                Description = message,
                Color = new DiscordColor(0xFF0000) // red
            };
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(true).AddEmbed(embed)).ConfigureAwait(true);
        }
    }
}
