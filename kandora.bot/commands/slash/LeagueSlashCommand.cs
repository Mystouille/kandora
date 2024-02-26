﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
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
            [Option(Resources.admin_seeLeagueRanking_displayInChat, Resources.admin_seeLeagueRanking_displayInChat_description)] bool displayInChat=false)
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
                var cutoffDate = league.FinalsCutoffDate;
                var logs = await RiichiCityService.Instance.GetAllTournamentLogs(league.Id);

                var teamsScoreDoubled = new Dictionary<int,int>();
                var userToTeam = new Dictionary<int, int>();
                foreach ( var team in teams)
                {
                    teamsScoreDoubled.Add(team.teamId, 0);
                }
                logs.ForEach(log => {
                    var isBeforeCutoff = cutoffDate == null ? false : log.Rounds[0].Hands[0].Time < cutoffDate;
                    var cutoffMultiplier = isBeforeCutoff ? 1 : 2;
                    var gameEndData = log.Rounds.Last().Hands.Where(hand => hand.EventType == EventType.GameEnd).First().Data.GameEndDataList;
                    gameEndData.ToImmutableList().ForEach(playerData =>
                    {
                        var teamId = 0;
                        if (!userToTeam.ContainsKey(playerData.UserId))
                        {
                            var userId = users.Values.Where(u => u.RiichiCityId == playerData.UserId).First().Id;
                            teamId = teamPlayers.Where(p => p.userId == userId).First().teamId;
                            userToTeam.Add(playerData.UserId, teamId);
                        } else
                        {
                            teamId = userToTeam[playerData.UserId];
                        }
                        teamsScoreDoubled[teamId] = teamsScoreDoubled[teamId] + playerData.FinalScore * cutoffMultiplier;
                    });
                });
                var rankingData = new List<(string, int)>();
                var sortedData = teamsScoreDoubled.Select(kv => (teams.Where(t=> t.teamId ==kv.Key).First().fancyName, ((float)(kv.Value))/2)).OrderByDescending(x => x.Item2);
                var sb = new StringBuilder();
                sb.AppendLine("Live ranking!");

                var cutoffHappened = cutoffDate == null ? false : cutoffDate < DateTime.UtcNow;
                if (!cutoffHappened)
                {
                    sortedData.ToList().ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2)}`  \t| {x.Item1}");
                    });
                } else {
                    sortedData.ToList().GetRange(0,4).ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2)}`  \t| {x.Item1}");
                    });
                    sb.AppendLine("\nEliminated:");
                    sortedData.ToList().GetRange(4, sortedData.Count() - 4).ForEach(x => {
                        sb.AppendLine($"`{formatScore(x.Item2*2)}`  \t| {x.Item1}");
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
            var floatScore = (float)(score) / 10;
            var absScore = floatScore > 0 ? floatScore : floatScore * -1;
            var stringScore = absScore.ToString("0.00");
            return $"`{(floatScore > 0 ? "+" : "-")}{stringScore.PadLeft(6,' ')}`";
        }

        
    }
}
