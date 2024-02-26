using System;
using System.Collections.Generic;
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
                var rcResp = await RiichiCityService.Instance.GetPlayerScores(league.Id);

                var rankingData = new Dictionary<int,(string,float)>();
                int nbPlayer = 0;
                teams.ForEach(team => rankingData.Add(team.teamId,(team.fancyName,0)));
                var cutOffMultiplier = cutoffDate != null ? 2 : 1;

                rcResp.Users.ToList().ForEach((player)=> {
                    var matchingUsers = users.Where(x=>x.Value.RiichiCityId == player.UserId);
                    if (matchingUsers.Count() == 0)
                    {
                        return;
                    }
                    var user = matchingUsers.First().Value;
                    var userId = user.Id;
                    var team = teamPlayers.Where(tp => tp.userId == userId);
                    if (team.Count() == 0)
                    {
                        return;
                    }
                    var teamId = team.First().teamId;
                    rankingData[teamId] = (rankingData[teamId].Item1, rankingData[teamId].Item2 + player.Score);
                    nbPlayer++;
                    Console.WriteLine($"team: {teamId}, user: {user.RiichiCityName}, score: {player.Score}");
                });
                Console.WriteLine($"total: {nbPlayer}");

                var sortedData = rankingData.Values.OrderByDescending(x => x.Item2);
                var sb = new StringBuilder();
                sb.AppendLine("Live ranking!");
                sortedData.ToList().ForEach(x => {
                    sb.AppendLine($"`{formatScore(x.Item2)}`  \t| {x.Item1}");
                });
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
            var stringScore = floatScore.ToString("0.0");
            return $"`{(floatScore > 0 ? "+" : "")}{stringScore.PadLeft(5,' ')}`";
        }

        
    }
}
