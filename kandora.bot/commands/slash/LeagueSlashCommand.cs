using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.http;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("league", Resources.league_groupDescription, defaultPermission: false)]
    class LeagueSlashCommands : KandoraSlashCommandModule
    {
        
        //[SlashCommand("startGame", Resources.admin_startGame_description)]
        public async Task StartGame(InteractionContext ctx,
            [Option(Resources.admin_startGame_user1, Resources.admin_startGame_user1_description)] string user1="",
            [Option(Resources.admin_startGame_user2, Resources.admin_startGame_user2_description)] string user2="",
            [Option(Resources.admin_startGame_user3, Resources.admin_startGame_user3_description)] string user3="",
            [Option(Resources.admin_startGame_user4, Resources.admin_startGame_user4_description)] string user4="")
        {
            try{
                var response = await RiichiCityService.Instance.GetTournamentPlayers(3220542);

                var rb = new DiscordInteractionResponseBuilder().WithContent(response).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeranking", Resources.admin_seeLeagueRanking_description)]
        public async Task SeeRanking(InteractionContext ctx,
            [Option(Resources.admin_seeLeagueRanking_displayInChat, Resources.admin_seeLeagueRanking_displayInChat_description)] bool displayInChat=false,
            [Option(Resources.admin_seeLeagueDelta_numberOfGames, Resources.admin_seeLeagueDelta_numberOfGames_description)] double nbGames = 0)
        {
            try{
                await ctx.DeferAsync(ephemeral: !displayInChat);

                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var leagues = LeagueDbService.GetLeaguesOnServer(serverDiscordId, onlyOngoing: true);
                var teams = LeagueDbService.GetLeagueTeams(leagues);
                var teamPlayers = LeagueDbService.GetLeaguePlayers(teams);
                if (leagues.Count() == 0)
                {
                    throw new Exception(Resources.commandError_leagueNotInitialized);
                }
                var league = leagues.First();

                var subList = LeagueDbService.GetSubs(league.Id);

                var cutoffDate = league.FinalsCutoffDate;
                var logs = await RiichiCityService.Instance.GetAllTournamentLogs(league.Id);

                var teamsScoreDoubled = new Dictionary<int, int>();
                var teamsDelta = new Dictionary<int, int>();
                var userToTeam = new Dictionary<string, int>();
                foreach ( var team in teams)
                {
                    teamsScoreDoubled.Add(team.teamId, 0);
                    teamsDelta.Add(team.teamId, 0);
                }
                var teamsAfterCutoff = new List<int>();

                logs.Sort(delegate (GameData a, GameData b) {
                    return b.Rounds[0].Hands[0].Time.Ticks.CompareTo(a.Rounds[0].Hands[0].Time.Ticks);
                });
                var latestGameIndex = 0;

                logs.ForEach(log => {
                    var isBeforeCutoff = cutoffDate == null ? false : log.Rounds[0].Hands[0].Time < cutoffDate;
                    var cutoffMultiplier = isBeforeCutoff ? 1 : 2;
                    var gameEndData = log.Rounds.Last().Hands.Where(hand => hand.EventType == EventType.GameEnd).First().Data.GameEndDataList;

                    gameEndData.ToImmutableList().ForEach(playerData =>
                    {
                        var teamId = 0;
                        var foundUsers = users.Values.Where(u => u.RiichiCityId == playerData.UserId || u.RiichiCitySecondaryId == playerData.UserId);
                        var userId = foundUsers.Any() ? foundUsers.First().Id : playerData.UserId.ToString();
                        var sub = subList.Where(s => s.gameId == log.GameId && s.inId == userId);
                        var actualPlayerId = sub.Count() == 0 ? userId : sub.First().outId;
                        var actualScore = sub.Count() == 0 ? playerData.FinalScore : -450;

                        if (!userToTeam.ContainsKey(actualPlayerId))
                        {
                            teamId = teamPlayers.Where(p => p.userId == actualPlayerId).First().teamId;
                            userToTeam.Add(actualPlayerId, teamId);
                        } else
                        {
                            teamId = userToTeam[actualPlayerId];
                        }

                        if (!isBeforeCutoff && !teamsAfterCutoff.Contains(teamId))
                        {
                            teamsAfterCutoff.Add(teamId);
                        }
                        if (latestGameIndex < nbGames)
                        {
                            teamsDelta[teamId] = teamsDelta[teamId] + actualScore;
                        }
                        teamsScoreDoubled[teamId] = teamsScoreDoubled[teamId] + actualScore * cutoffMultiplier;
                    });
                    latestGameIndex++;
                });
                var rankingData = new List<(string, int)>();
                var sortedData = teamsScoreDoubled.Select(kv => (teams.Where(t => t.teamId == kv.Key).First().fancyName, ((float)(kv.Value)) / 2, teamsDelta[kv.Key])).OrderByDescending(x => x.Item2);
                var finalistData = teamsScoreDoubled.Where(ts=>teamsAfterCutoff.Contains(ts.Key)).Select(kv => (teams.Where(t => t.teamId == kv.Key).First().fancyName, ((float)(kv.Value)) / 2, teamsDelta[kv.Key])).OrderByDescending(x => x.Item2);
                var sb = new StringBuilder();
                sb.AppendLine("Live ranking!");

                var cutoffHappened = cutoffDate == null ? false : cutoffDate < DateTime.UtcNow;
                if (!cutoffHappened)
                {
                    sortedData.ToList().GetRange(0, 4).ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2 * 2)}`\t| {formatDelta(x.Item3, nbGames>0)}  {x.Item1}");
                    });

                    sb.AppendLine("\n");
                    sortedData.ToList().GetRange(4, sortedData.Count() - 4).ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2 * 2)}`\t| {formatDelta(x.Item3, nbGames > 0)}  {x.Item1}");
                    });
                } else {
                    finalistData.ToList().GetRange(0, 4).ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2)}`\t| {formatDelta(x.Item3, nbGames > 0)}  {x.Item1}");
                    });
                }
                var wb = new DiscordWebhookBuilder().WithContent(sb.ToString());
                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                var wb = new DiscordWebhookBuilder().WithContent(e.Message);
                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
            }
        }

        private string formatScore(float score)
        {
            if (score == 0)
            {
                return $"`{"0".PadLeft(7, ' ')}`";
            }
            var floatScore = (float)(score) / 10;
            var absScore = floatScore > 0 ? floatScore : floatScore * -1;
            var stringScore = absScore.ToString("0.0");
            return $"`{(floatScore > 0 ? "+" : "-")}{stringScore.PadLeft(6, ' ')}`";
        }

        private string formatDelta(int score, bool displayDelta)
        {
            if (!displayDelta)
            {
                return "";
            }
            if (score == 0)
            {
                return $"`{"".PadLeft(7, ' ')}`\t|";
            }
            var floatScore = (float)(score) / 10;
            var absScore = floatScore > 0 ? floatScore : floatScore * -1;
            var stringScore = absScore.ToString("0.0");
            return $"`{(floatScore > 0 ? "+" : "-")}{stringScore.PadLeft(6, ' ')}`\t|";
        }


    }
}
