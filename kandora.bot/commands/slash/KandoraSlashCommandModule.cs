using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.resources;
using kandora.bot.services.discord;
using System;

namespace kandora.bot.commands.slash
{
    public class KandoraSlashCommandModule : ApplicationCommandModule
    {
        protected static KandoraSlashContext kandoraContext = KandoraSlashContext.Instance;

        protected async void replyWithException(InteractionContext ctx, Exception e)
        {
            try
            {
                var description = e.Message + "\n" + e.StackTrace;
                var embed = new DiscordEmbedBuilder
                {
                    Title = Resources.commandError_title,
                    Description = description,
                    Color = new DiscordColor(0xFF0000) // red
                };
                var rb = new DiscordInteractionResponseBuilder().AsEphemeral(true).AddEmbed(embed);


                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch(Exception e2)
            {
                // when the response has already been posted and code failed after (like when creating reactions)
                await ctx.DeleteResponseAsync().ConfigureAwait(true);

            }
        }
    }
}
