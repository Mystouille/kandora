using System.Threading.Tasks;
using DSharpPlus;
using System.Configuration;
using Microsoft.Extensions.Logging;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using kandora.bot.services.discord;
using DSharpPlus.SlashCommands;
using kandora.bot.commands.slash;

namespace kandora.bot
{
    class Kandora
    {
        static DiscordClient client;
        static SlashCommandsExtension slashCommands;

        static void Main()
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var token = ConfigurationManager.AppSettings.Get("ClientToken");
            client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigurationManager.AppSettings.Get("ClientToken"),
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });
            client.UseInteractivity(new InteractivityConfiguration());

            slashCommands = client.UseSlashCommands();
            slashCommands.RegisterCommands<QuizzSlashCommands>();
            slashCommands.RegisterCommands<MahjongSlashCommands>();
            slashCommands.RegisterCommands<AdminSlashCommands>();
            slashCommands.RegisterCommands<LeagueSlashCommands>();
            //nope
            //slashCommands.RegisterCommands<TestSlashCommands>();

            client.MessageReactionAdded += ReactionListener.OnReactionAdded;
            client.MessageReactionRemoved += ReactionListener.OnReactionRemoved;

            await client.ConnectAsync();

            await Task.Delay(-1);
        }

    }
}
