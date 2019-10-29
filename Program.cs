using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Configuration;

namespace Kandora
{
    class Program
    {
        static DiscordClient client;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }



        static async Task MainAsync(string[] args)
        {
            client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigurationManager.AppSettings.Get("ClientToken"),
                TokenType = TokenType.Bot
            });

            client.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("!hand"))
                {
                    var messageSplit = e.Message.Content.Split("!hand");
                    var hand = messageSplit.Length>0 ? messageSplit[messageSplit.Length - 1] : "";
                    await e.Message.RespondAsync(HandParser.GetHandEmojiCode(hand, client));
                }
            };

            await client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
