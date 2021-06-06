using kandora.bot.http;
using Newtonsoft.Json;
using System;

namespace kandora.bot
{
    class TestClient
    {
        static void Main(string[] args)
        {
            var game = JsonConvert.DeserializeObject<RiichiGame>(Samples.tenhouLog);
            Console.WriteLine(game.Title[0]);
        }
    }
}
