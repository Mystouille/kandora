using kandora.bot.http;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.client
{
    class TensoulClient
    {
        static HttpClient client = new HttpClient();

        public static async Task<TenhouGame> GetMahjsoulLog(string logId, int lang)
        {
            var url = $"http://chinesecartoons.club/convert/?id={logId}&lang={lang}";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                return TenhouLogParser.ParseTenhouGame(payload);
            }
            return null;
        }
    }
}
