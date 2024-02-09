using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.http;
using Microsoft.VisualBasic;
using static System.Formats.Asn1.AsnWriter;

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
                var rb = new DiscordInteractionResponseBuilder().WithContent("fetching data...").AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var league = LeagueDbService.GetOngoingLeagueOnServer(serverDiscordId);
                var teams = LeagueDbService.GetLeagueTeams(league.Id);
                if (league == null)
                {
                    throw new Exception(Resources.commandError_leagueNotInitialized);
                }
                var rcResp = await RiichiCityService.Instance.GetPlayerScores(league.Id);

                float score = rcResp.Users[0].Score/10;

                if(!displayInChat){
                    rb = rb.AsEphemeral();
                }
                var wb = new DiscordWebhookBuilder().WithContent(score.ToString());
                await ctx.EditResponseAsync(wb);
            }
            catch (Exception e)
            {
                var wb = new DiscordWebhookBuilder().WithContent(e.Message);
                await ctx.EditResponseAsync(wb);
            }
        }

        
    }
}
