using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using kandora.bot.resources;

namespace kandora.bot.services.http
{
    public sealed class RiichiCityService
    {
        private string deviceId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private string domainId = "alicdn.mahjong-jp.net";
        private string apiVersion = "1.1.4.11030";
        private string sessionId = "";
        private int userId = -1;
        private string email = ConfigurationManager.AppSettings.Get("RiichiCityLogin");
        private string password = ConfigurationManager.AppSettings.Get("RiichiCityPassword");


        private HttpClient client;
        private static readonly RiichiCityService instance = new RiichiCityService();
        static RiichiCityService()
        {
        }

        private RiichiCityService()
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0,0,6);
        }

        
        public static RiichiCityService Instance
        {
            get
            {
                return instance;
            }
        }

        private async Task<string> InitSession(){
            var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://{domainId}/users/initSession"),
                    Headers = { 
                        { "Cookies", $"{{\"channel\":\"default\",\"deviceid\":\"{deviceId}\",\"lang\":\"en\",\"version\":\"{apiVersion}\",\"platform\":\"pc\"}}" }
                    },
                };

            var response = await client.SendAsync(httpRequestMessage);
            
            var payload = await response.Content.ReadAsStringAsync();
            var SID = JsonSerializer.Deserialize<InitSessionResponse>(payload).SID;
            sessionId = SID;
            return SID;
        }

        private async Task<(int,string)> Login()
        {
            client.DefaultRequestHeaders.Clear();
            await InitSession();

            var cookies = $"{{\"channel\":\"default\",\"deviceid\":\"{deviceId}\",\"lang\":\"en\",\"version\":\"{apiVersion}\",\"platform\":\"pc\",\"sid\":\"{sessionId}\"}}";
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://{domainId}/users/emailLogin"),
                Headers = {
                        { "Cookies", cookies }
                    },

                Content = new StringContent($"{{ \"passwd\":\"{password}\",\"email\":\"{email}\"}}"),
            };

            var response = await client.SendAsync(httpRequestMessage);

            var payload = await response.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<LoginResponse>(payload);

            if(parsed.Code != 0)
            {
                throw new Exception(Resources.commandError_RiichiCityConnectionFailed);
            }
            userId = parsed.Data.User.Id;

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "UnityPlayer/2019.4.23f1 (UnityWebRequest/1.0, libcurl/7.52.0-DEV)");
            client.DefaultRequestHeaders.Add("Cookies", $"{{\"channel\":\"default\",\"deviceid\":\"{deviceId}\",\"lang\":\"en\",\"version\":\"{apiVersion}\",\"platform\":\"pc\",\"region\":\"cn\",\"sid\":\"{sessionId}\",\"uid\":{userId}}}");
            client.DefaultRequestHeaders.Add("X-Unity-Version", "2019.4.23f1");

            return (parsed.Code, parsed.Message);
        }

        public async Task<string> GetTournamentPlayers(int tournamentId)
        {
            await Login();

            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/lobbys/getSelfManageInfo",
                new StringContent(content: $"{{\"matchID\":{tournamentId}}}", encoding: null, mediaType: "application/json")
            );

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<RankResponse> GetPlayerScores(int tournamentId)
        {
            await Login();

            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/stats/getSelfRank",
                new StringContent(content: $"{{\"matchID\":{tournamentId}}}", encoding: null, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<RankResponse>(payload);
            return parsed;
            
        }

    }
}