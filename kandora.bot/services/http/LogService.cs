using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kandora.bot.services.http
{
    public sealed class LogService
    {
        private static readonly LogService instance = new LogService();
        private HttpClient tenhouLogClient;
        private HttpClient mahjsoulLogClient;
        private static Regex tenhouRegex = new Regex(@"^[0-9]{10}gm-[0-9]{4}-[0-9]{4}-[0-9a-f]{8}$");
        //useless because of obfuscated logIds
        private static Regex mahjsoulRegex = new Regex(@"^[0-9]{6}-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");

        static LogService()
        {
        }

        private LogService()
        {
            tenhouLogClient = new HttpClient();
            tenhouLogClient.Timeout = new TimeSpan(0,0,6);
            tenhouLogClient.DefaultRequestHeaders.Add("Referer", "https://tenhou.net/");
            mahjsoulLogClient = new HttpClient();
            mahjsoulLogClient.Timeout = new TimeSpan(0, 0, 6);
        }

        public static LogService Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<RiichiGame> GetLog(string logId, int lang)
        {
            string logIdSanitized = logId.Replace(" ", "");
            if (tenhouRegex.IsMatch(logId))
            {
                var log = await GetTenhouLog(logId);
                // retry once just in case there where login issues
                if(log == null)
                {
                    log = await GetTenhouLog(logId);
                }
                return TenhouLogParser.ParseTenhouFormatGame(log, GameType.Tenhou);
            }
            else
            {
                var log = await GetMahjsoulLogAsTenhou(logId, lang);
                // retry once just in case there where login issues
                if (log == null)
                {
                    log = await GetMahjsoulLogAsTenhou(logId, lang);
                }

                return TenhouLogParser.ParseTenhouFormatGame(log, GameType.Mahjsoul);
            }
        }

        private async Task<string> GetTenhouLog(string logId)
        {
            var url = $"https://tenhou.net/5/mjlog2json.cgi?{logId}";
            HttpResponseMessage response = await tenhouLogClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private async Task<string> GetMahjsoulLogAsTenhou(string logId, int lang)
        {
            var url = $"http://chinesecartoons.club/convert?id={logId}&lang={lang}";
            HttpResponseMessage response = await mahjsoulLogClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            throw new Exception($"statusCode:{response.StatusCode}, reason:{response.ReasonPhrase}, content:{response.Content}");
        }
    }
}
