using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Configuration;
using DSharpPlus.CommandsNext;

namespace Kandora
{
    class Kandora
    {
        static DiscordClient client;
        static CommandsNextModule commands;

        static void Main()
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigurationManager.AppSettings.Get("ClientToken"),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });


            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<KandoraCommands>();

            await client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
