﻿using System.Threading.Tasks;
using DSharpPlus;
using System.Configuration;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using kandora.bot.commands;
using kandora.bot.services.http;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using System;

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
            client.UseInteractivity(new InteractivityConfiguration());


            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[1] { "!" }
            });

            commands.RegisterCommands<MahjongCommands>();
            commands.RegisterCommands<RankingCommands>();
            commands.RegisterCommands<UserCommands>();

            client.MessageReactionAdded += Listeners.OnReactionAdded;
            client.MessageReactionRemoved += Listeners.OnReactionRemoved;

            await client.ConnectAsync();

            await Task.Delay(-1);
        }

    }
}
