using System.Threading.Tasks;
using DSharpPlus;
using System.Configuration;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using kandora.bot.commands;
using Newtonsoft.Json;
using kandora.bot.http;
using System;
using kandora.bot.utils;
using kandora.bot.client;

namespace kandora.bot
{
    class Kandora
    {
        static DiscordClient client;
        static CommandsNextExtension commands;

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
                MinimumLogLevel = LogLevel.Debug
            });


            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[1] { "!" }
            });

            commands.RegisterCommands<MahjongCommands>();
            commands.RegisterCommands<RankingCommands>();
            commands.RegisterCommands<UserCommands>();

            await client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
