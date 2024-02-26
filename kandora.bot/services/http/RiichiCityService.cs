using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using kandora.bot.resources;
using kandora.bot.utils;
using kandora.bot.utils.RiichiCityParser;

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
            client.Timeout = new TimeSpan(0, 0, 6);
        }


        public static RiichiCityService Instance
        {
            get
            {
                return instance;
            }
        }


        private static Random rng = new Random();

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private async Task<string> InitSession()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://{domainId}/users/initSession"),
                Headers = {
                        { "Cookies", $"{{\"channel\":\"default\",\"deviceid\":\"{deviceId}\",\"lang\":\"en\",\"version\":\"{apiVersion}\",\"platform\":\"pc\"}}" }
                    },
            };

            var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(true); ;

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
            var SID = JsonSerializer.Deserialize<InitSessionResponse>(payload).SID;
            sessionId = SID;
            return SID;
        }

        private async Task<(int, string)> Login()
        {
            client.DefaultRequestHeaders.Clear();
            await InitSession().ConfigureAwait(true); ;

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

            var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(true); ;

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
            var parsed = JsonSerializer.Deserialize<LoginResponse>(payload);

            if (parsed.Code != 0)
            {
                throw new Exception(Resources.commandError_RiichiCityConnectionFailed);
            }
            userId = parsed.Data.User.Id;

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "UnityPlayer/2019.4.23f1 (UnityWebRequest/1.0, libcurl/7.52.0-DEV)");
            client.DefaultRequestHeaders.Add("Cookies", $"{{\"channel\":\"default\",\"deviceid\":\"{deviceId}\",\"lang\":\"en\",\"version\":\"{apiVersion}\",\"platform\":\"pc\",\"region\":\"cn\",\"sid\":\"{sessionId}\",\"uid\":{userId}}}");
            client.DefaultRequestHeaders.Add("X-Unity-Version", "2019.4.23f1");
            //client.DefaultRequestHeaders.Add("X-Unity-Version: ", "2020.3.42f1c1");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate,gzip");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return (parsed.Code, parsed.Message);
        }

        public async Task<string> GetTournamentPlayers(int tournamentId)
        {
            await Login().ConfigureAwait(true); ;

            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/lobbys/getSelfManageInfo",
                new StringContent(content: $"{{\"matchID\":{tournamentId}}}", encoding: null, mediaType: "application/json")
            ).ConfigureAwait(true); ;

            return await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
        }

        public async Task<TournamentInfoResponse> GetTournamentInfo(int tournamentId, bool fallBack = true)
        {
            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/lobbys/enterSelfBuild",
                new StringContent(content: $"{{\"id\":{tournamentId}}}", encoding: null, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
            var parsed = JsonSerializer.Deserialize<TournamentInfoResponse>(payload);

            if (parsed.Code == 10 && fallBack)
            {
                await Login().ConfigureAwait(true); ;
                return await GetTournamentInfo(tournamentId, fallBack: false).ConfigureAwait(true); ;
            }
            return parsed;
        }

        public async Task<RankResponse> GetPlayerScores(int tournamentId, bool fallBack = true)
        {
            var tournamentInfo = await GetTournamentInfo(tournamentId).ConfigureAwait(true);

            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/stats/getSelfRank",
                new StringContent(content: $"{{\"matchID\":{tournamentId},\"classifyID\":\"{tournamentInfo.Data.ClassifyId}\"}}", encoding: null, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
            var parsed = JsonSerializer.Deserialize<RankResponse>(payload);

            if (parsed.Code == 10 && fallBack)
            {
                await Login().ConfigureAwait(true); ;
                return await GetPlayerScores(tournamentId, fallBack: false).ConfigureAwait(true); ;
            }
            return parsed;
        }

        public async Task<PlayerStatusResponse> GetPlayersStatus(int tournamentId, bool fallBack = true)
        {
            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/lobbys/getSelfManageInfo",
                new StringContent(content: $"{{\"matchID\":{tournamentId}}}", encoding: Encoding.UTF8, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var parsed = JsonSerializer.Deserialize<PlayerStatusResponse>(payload);

            if (parsed.Code == 10 && fallBack)
            {
                await Login().ConfigureAwait(true); ;
                return await GetPlayersStatus(tournamentId, fallBack: false).ConfigureAwait(true); ;
            }
            return parsed;
        }

        public async Task<bool> StartGame(int tournamentId, List<int> playerIds, bool fallBack = true)
        {
            var botIds = new List<int> { 113808489, 217163646, 511575033 };
            var ids = new List<int>(playerIds);
            if (ids.Count < 4)
            {
                ids.AddRange(botIds.GetRange(0, 4 - ids.Count));
            }

            Shuffle(ids); // Mix up the order because the first element is always starting East, and so on.

            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/lobbys/allocateSelfUser",
                new StringContent(content: $"{{\"matchID\":{tournamentId},\"usersID\":[{string.Join(",", ids)}],\"table_idx\":1}}", encoding: Encoding.UTF8, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var parsed = JsonSerializer.Deserialize<StartGameResponse>(payload);

            if (parsed.Code == 10 && fallBack)
            {
                await Login().ConfigureAwait(true); ;
                return await StartGame(tournamentId, playerIds, fallBack: false).ConfigureAwait(true); ;
            }
            return parsed.IsSuccess;
        }



        public async Task<GameData> GetLog(string gameId, bool fallBack = true)
        {
            if (fallBack && FileUtils.IsLogCached(gameId))
            {
                Console.WriteLine($"from file");
                return JsonSerializer.Deserialize<GameResponse>(FileUtils.ReadLog(gameId)).Data;
            }
            else
            {

                Console.WriteLine($"from web");
                HttpResponseMessage response = await client.PostAsync(
                    $"https://{domainId}/record/getRoomData",
                    new StringContent(content: $"{{\"keyValue\":\"{gameId}\",\"isObserve\":false}}", encoding: Encoding.UTF8, mediaType: "application/json")
                );

                var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                var parsed = JsonSerializer.Deserialize<GameResponse>(payload);

                if (parsed.Code == 10 && fallBack)
                {
                    await Login().ConfigureAwait(true); ;
                    return await GetLog(gameId, fallBack: false).ConfigureAwait(true);
                }
                FileUtils.SaveLog(gameId, payload);
                return parsed.Data;
            }
        }

        public async Task<List<GameData>> GetAllTournamentLogs(int tournamentId, int skip = 0, bool fallBack = true)
        {
            // Getting the logIds
            var tournamentInfo = await GetTournamentInfo(tournamentId).ConfigureAwait(true);
            var skipValue = skip > 0 ? $", \"skip\":{skip}" : "";
            HttpResponseMessage response = await client.PostAsync(
                $"https://{domainId}/record/readPaiPuList",
                new StringContent(content: $"{{\"startTime\":0, \"endTime\":0{skipValue}, \"classifyID\":\"{tournamentInfo.Data.ClassifyId}\", \"isSelf\":true, \"gamePlay\":1002}}", encoding: Encoding.UTF8, mediaType: "application/json")
            );

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var parsed = JsonSerializer.Deserialize<LogListResponse>(payload);

            if (parsed.Code == 10 && fallBack)
            {
                await Login().ConfigureAwait(true); ;
                return await GetAllTournamentLogs(tournamentId, skip, fallBack: false).ConfigureAwait(true);
            }
            var relevantLogs = parsed.Data.Where(x=>x.IsIgnored==false).Select(x=>x.LogId);

            // Fetching the logs
            var logList = new List<GameData>();
            foreach (var logId in relevantLogs)
            {
                Console.WriteLine($"fetching log {logId}");
                var log = await GetLog(logId).ConfigureAwait(true);
                logList.Add(log);
            }
            return logList;
        }
    }
}