using System.Threading.Tasks;
using DSharpPlus;
using System.Configuration;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using kandora.bot.commands.regular;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using kandora.bot.services.discord;
using DSharpPlus.SlashCommands;
using kandora.bot.commands.slash;
using DSharpPlus.Entities;

namespace kandora.bot
{
    class Kandora
    {
        static DiscordClient client;
        static CommandsNextExtension commands;
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


            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[1] { "!" }
            });
            slashCommands = client.UseSlashCommands();


            commands.RegisterCommands<MahjongCommands>();
            commands.RegisterCommands<RankingCommands>(); 
            commands.RegisterCommands<UserCommands>();
            commands.RegisterCommands<LeagueConfigCommands>();
            commands.RegisterCommands<InitCommands>();

            ulong guildId = 984913976544616479; //KandoraHome discord guild id
            //slashCommands.RegisterCommands<QuizzSlashCommands>(guildId);
            //slashCommands.RegisterCommands<MahjongSlashCommands>(guildId);
            slashCommands.RegisterCommands<AdminSlashCommands>(guildId);
            slashCommands.RegisterCommands<LeagueSlashCommands>(guildId);

            ulong tntGuildId = 665504390181945344; //TNT discord guild
            //slashCommands.RegisterCommands<MahjongSlashCommands>(tntGuildId);
            //slashCommands.RegisterCommands<QuizzSlashCommands>(tntGuildId);

            ulong antreTuilesId = 625098212432412682; //Antre des tuiles
            //slashCommands.RegisterCommands<MahjongSlashCommands>(antreTuilesId);
            //slashCommands.RegisterCommands<QuizzSlashCommands>(antreTuilesId); 

            client.MessageReactionAdded += ReactionListener.OnReactionAdded;
            client.MessageReactionRemoved += ReactionListener.OnReactionRemoved;

            await client.ConnectAsync();

            await Task.Delay(-1);
        }

    }
}
