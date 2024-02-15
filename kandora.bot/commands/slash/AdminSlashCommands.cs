using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using kandora.bot.mahjong;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.db;
using kandora.bot.services.http;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("admin", Resources.admin_groupDescription, defaultPermission: false)]
    class AdminSlashCommands : KandoraSlashCommandModule
    {
        private string GetHandMessage(IEnumerable<DiscordEmoji> emojis)
        {
            string toReturn = "";
            var lastEmoji = "";
            foreach (var emoji in emojis)
            {
                toReturn += lastEmoji;
                lastEmoji = emoji;
            }
            toReturn += (emojis.Count() > 12 ? " " : "") + lastEmoji;
            return toReturn;
        }

        [SlashCommand("test", "test fetch log")]
        public async Task Test(InteractionContext ctx)
        {
            try
            {
                await ctx.DeferAsync(ephemeral: true).ConfigureAwait(true);

                var data = await RiichiCityService.Instance.GetLog("cn5872m9nc70otuhfm5g");
                var gameData = data.Data;
                var firstHand = gameData.Rounds[0].Hands[0].Data.StartingHand.ToList();
                var emojiList= HandParser.GetHandEmojiCodes(string.Join("",firstHand), ctx.Client, sorted:true);
                var sb = new StringBuilder();
                sb.AppendLine(GetHandMessage(emojiList));
                sb.AppendLine(gameData.Rounds[0].Hands[5].Data.Action.ToString());
                sb.AppendLine(gameData.NowTime.ToString());
                sb.AppendLine(gameData.Rounds[0].Hands[0].Time.ToString());
                var wb = new DiscordWebhookBuilder().WithContent(sb.ToString());
                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                var wb = new DiscordWebhookBuilder().WithContent(e.Message + "\n" + e.StackTrace);
                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
            }

        }
            [SlashCommand("startGame", Resources.admin_startGame_description)]
        public async Task StartGame(InteractionContext ctx, 
            [Option(Resources.admin_startGame_user1, Resources.admin_startGame_user1_description)] string user1,
            [Option(Resources.admin_startGame_user2, Resources.admin_startGame_user2_description)] string user2,
            [Option(Resources.admin_startGame_user3, Resources.admin_startGame_user3_description)] string user3,
            [Option(Resources.admin_startGame_user4, Resources.admin_startGame_user4_description)] string user4)
        {
            try
            {
                await ctx.DeferAsync(ephemeral: true).ConfigureAwait(true);

                var userIds = new List<(string, string)>
                {
                    (getIdFromPlayerParam(user1), user1),
                    (getIdFromPlayerParam(user2), user2),
                    (getIdFromPlayerParam(user3), user3),
                    (getIdFromPlayerParam(user4), user4)
                };

                var serverDiscordId = ctx.Guild.Id.ToString();

                var status = await computeStatus(userIds, serverDiscordId, ctx.Client).ConfigureAwait(true);

                var refreshId = Guid.NewGuid().ToString("N");
                var startId = Guid.NewGuid().ToString("N");
                DiscordButtonComponent refreshButton = new DiscordButtonComponent(ButtonStyle.Secondary, refreshId, "Refresh");
                DiscordButtonComponent startButton = new DiscordButtonComponent(ButtonStyle.Primary, startId, "Start Game!", disabled: !status.Item2);

                ctx.Client.ComponentInteractionCreated += async (client, eventArgs) =>
                {

                    if (eventArgs.Interaction.Data.CustomId == refreshId)
                    {
                        try
                        {
                            var status = await computeStatus(userIds, serverDiscordId, client).ConfigureAwait(true);

                            if (status.Item2)
                            {
                                startButton.Enable();
                            }
                            else
                            {
                                startButton.Disable();
                            }
                            var webhookBuilder = new DiscordWebhookBuilder()
                                .WithContent(status.Item1).AddComponents(refreshButton, startButton);

                            await ctx.EditResponseAsync(webhookBuilder);

                        }
                        catch (Exception e)
                        {
                            var wb = new DiscordWebhookBuilder().WithContent(e.Message);
                            await ctx.EditResponseAsync(wb).ConfigureAwait(true);
                        };
                    }
                    else if (eventArgs.Interaction.Data.CustomId == startId)
                    {
                        try
                        {
                            
                            var users = UserDbService.GetUsers();
                            var servers = ServerDbService.GetServers(users);
                            var leagues = LeagueDbService.GetLeaguesOnServer(serverDiscordId, onlyOngoing: true);
                            var teams = LeagueDbService.GetLeagueTeams(leagues);
                            var teamPlayers = LeagueDbService.GetLeaguePlayers(teams);
                            var league = leagues.First();
                            var rcIds = new List<int>();
                            userIds.ForEach(x =>
                            {
                                if (!users.ContainsKey(x.Item1))
                                {
                                    throw new Exception(string.Format(Resources.commandError_PlayerUnknown, x.Item2));
                                }
                                if (users[x.Item1].RiichiCityId < 0)
                                {
                                    throw new Exception(string.Format(Resources.commandError_PlayerNotOnRiichiCity, x.Item2));
                                }
                                if (!teamPlayers.Select(player => player.userId).Contains(x.Item1))
                                {
                                    throw new Exception(string.Format(Resources.commandError_PlayerNotInAteam, x.Item2));
                                }
                                rcIds.Add(users[x.Item1].RiichiCityId);
                            });

                            var isOK = await RiichiCityService.Instance.StartGame(league.Id, rcIds);

                            if (isOK)
                            {
                                var wb = new DiscordWebhookBuilder().WithContent(Resources.admin_startGame_gameStarted);
                                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
                            }
                            else
                            {
                                var status = await computeStatus(userIds, serverDiscordId, client).ConfigureAwait(true);
                                var wb = new DiscordWebhookBuilder().WithContent(status.Item1).AddComponents(refreshButton, startButton);
                                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
                            }
                        }
                        catch (Exception e)
                        {
                            
                            var wb = new DiscordWebhookBuilder().WithContent(e.Message);
                            await ctx.EditResponseAsync(wb).ConfigureAwait(true);
                        };
                    }
                };
 
                var webhookBuilder = new DiscordWebhookBuilder()
                    .WithContent(status.Item1).AddComponents(refreshButton, startButton);

                await ctx.EditResponseAsync(webhookBuilder);
            }
            catch (Exception e)
            {
                var wb = new DiscordWebhookBuilder().WithContent(e.Message);
                await ctx.EditResponseAsync(wb).ConfigureAwait(true);
            }
        }

        private async Task<(string,bool)> computeStatus(List<(string, string)> userIds, string serverId, DiscordClient client )
        {
            var users = UserDbService.GetUsers();
            var servers = ServerDbService.GetServers(users);
            var leagues = LeagueDbService.GetLeaguesOnServer(serverId, onlyOngoing: true);
            var teams = LeagueDbService.GetLeagueTeams(leagues);
            var teamPlayers = LeagueDbService.GetLeaguePlayers(teams);
            if (leagues.Count() == 0)
            {
                throw new Exception(Resources.commandError_leagueNotInitialized);
            }

            var fullIds = new List<(int, string)>(); // (rcId, fullname)
            userIds.ForEach(x =>
            {
                if (!users.ContainsKey(x.Item1))
                {
                    throw new Exception(string.Format(Resources.commandError_PlayerUnknown, x.Item2));
                }
                if (users[x.Item1].RiichiCityId < 0)
                {
                    throw new Exception(string.Format(Resources.commandError_PlayerNotOnRiichiCity, x.Item2));
                }
                if (!teamPlayers.Select(player => player.userId).Contains(x.Item1))
                {
                    throw new Exception(string.Format(Resources.commandError_PlayerNotInAteam, x.Item2));
                }
                fullIds.Add((users[x.Item1].RiichiCityId, x.Item2));
            });

            if (leagues.Count() > 1)
            {
                throw new Exception("whoops, more than one league active");
            }

            var league = leagues.First();

            var statusList = (await RiichiCityService.Instance.GetPlayersStatus(league.Id)).Data;
            var fullData = new List<(string, PlayerStatusData)>();
            fullIds.ForEach(tuple =>
            {
                var status = statusList.Where(status => status.UserId == tuple.Item1);
                if (status.Count() == 0)
                {
                    //throw new Exception("Player status not found from RC data");
                }
                fullData.Add((tuple.Item2, status.FirstOrDefault()));
            });

            var sb = new StringBuilder();
            var okStatus = DiscordEmoji.FromName(client, ":white_check_mark:");
            var koStatus = DiscordEmoji.FromName(client, ":octagonal_sign:");
            sb.AppendLine("Players status:");
            fullData.ToList().ForEach(x => {
                var rcName = x.Item2 != null ? x.Item2.Name : "???";
                var statusStr = x.Item2 != null ? (x.Item2.Status == 2 ? okStatus : koStatus): koStatus;
                sb.AppendLine($"{statusStr}\t{x.Item1}({rcName})");
            });

            var allOk = fullData.Where(x => x.Item2!= null && x.Item2.Status == 2).Count() == fullData.Count();

            return (sb.ToString(), allOk);
        }

        [SlashCommand("startLeaderboard", Resources.admin_startLeaderboard_description)]
        public async Task StartLeaderboard(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name);
                }
                else
                {
                    if (servers[serverDiscordId].LeaderboardConfigId != null)
                    {
                        throw new Exception(Resources.commandError_leaderboardAlreadyInitialized);
                    }
                }
                var leaderboardConfigId = ConfigDbService.CreateConfig();
                ServerDbService.StartLeaderboardOnServer(serverDiscordId, leaderboardConfigId);
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_startLeaderboard_leaderboardStarted, ctx.Guild.Name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("startLeague", Resources.admin_startLeague_description)]
        public async Task StartLeague(InteractionContext ctx,
            [Option(Resources.admin_startLeague_tournamentId, Resources.admin_startLeague_tournamentId_description)] long tournamentId,
            [Option(Resources.admin_startLeague_displayName, Resources.admin_startLeague_displayName_description)] string displayName)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var leagues = LeagueDbService.GetLeaguesOnServer(serverDiscordId, onlyOngoing: true);
                var teams = LeagueDbService.GetLeagueTeams(leagues);
                var teamPlayers = LeagueDbService.GetLeaguePlayers(teams);
                if (leagues.Count() != 0)
                {
                    throw new Exception(Resources.commandError_leagueAlreadyInitialized);
                }
                if (!servers.ContainsKey(serverDiscordId))
                {
                    ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name);
                }

                LeagueDbService.StartLeague(serverDiscordId, (int)tournamentId, displayName);

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_startLeague_leagueStarted, ctx.Guild.Name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("createTeam", Resources.admin_createTeam_description)]
        public async Task CreateTeam(InteractionContext ctx,
            [Option(Resources.admin_createTeam_teamName, Resources.admin_createTeam_teamName_description)] string teamName,
            [Option(Resources.admin_createTeam_fancyName, Resources.admin_createTeam_fancyName_description)] string fancyNameParam = "")
        {
            try
            {
                var fancyName = fancyNameParam == "" ? teamName : fancyNameParam;
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

                if (teams.Where(x=>x.name == teamName).Count() > 0)
                {
                    throw new Exception(Resources.commandError_teamNameAlreadyExists);
                }

                LeagueDbService.CreateLeagueTeam(league.Id, teamName, fancyName);

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_createTeam_teamCreated, teamName ,league.DisplayName)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("addTeamPlayer", Resources.admin_addPlayer_description)]
        public async Task AddTeamPlayer(InteractionContext ctx,
            [Option(Resources.admin_addPlayer_nickname, Resources.admin_addPlayer_nickname_description)] string userName,
            [Option(Resources.admin_addTeamPlayer_team, Resources.admin_addTeamPlayer_team_description)] string teamName)
        {
            try
            {

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

                var userId = getIdFromPlayerParam(userName);

                var server = servers[serverDiscordId]; 
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                //Check if user exists
                if (!server.Users.Select(x => x.Id).Contains(userId))
                {
                    throw new Exception(string.Format(Resources.commandError_PlayerUnknown,userName));
                }

                var user = server.Users.Where(x => x.Id == userId).First();

                if(user.RiichiCityId == null || user.RiichiCityId <= 0)
                {
                    throw new Exception(string.Format(Resources.commandError_PlayerNotOnRiichiCity, userName));
                }

                if (league == null)
                {
                    throw new Exception(Resources.commandError_leagueNotInitialized);
                }

                if(teams.Where(x=>x.name == teamName).Count() == 0)
                {
                    throw new Exception(string.Format(Resources.commandError_teamDoesNotExist, teamName));
                }
                var team = teams.Where(x=>x.name == teamName).First();

                if(teamPlayers.Where(x=>x.userId == userId).Count() > 0)
                {
                    throw new Exception(Resources.commandError_playerAlreadyPresentInATeam);
                }

                var isCaptain = teamPlayers.Where(x=>x.teamId == team.teamId).Count() == 0;

                LeagueDbService.AddPlayerToTeam(userId, team.teamId, isCaptain);

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_addTeamPlayer_userHasBeenAdded, userName, team.fancyName)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        //[SlashCommand("updateDisplayNames", "updateDisplayNames")]
        public async Task UpdateNames(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var nbUpdated = 0;
                var discordUsers = await ctx.Guild.GetAllMembersAsync();
                foreach (var user in users){
                    if(user.Value.DisplayName != null && user.Value.DisplayName.Length > 0)
                    {
                        continue;
                    }
                    var discordUser = discordUsers.Where(x => x.Id.ToString() == user.Key).FirstOrDefault();
                    if(discordUser != null)
                    {
                        ServerDbService.SetUserDisplayName(serverDiscordId, user.Key, discordUser.DisplayName);
                        nbUpdated++;
                    }
                }
                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format("Player display names updated: {0}", nbUpdated)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("flushServer", Resources.admin_flushServer_description)]
        public async Task EndLeaderboard(InteractionContext ctx)
        {
            try
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var leagues = LeagueDbService.GetLeaguesOnServer(serverDiscordId);
                var teams = LeagueDbService.GetLeagueTeams(leagues);
                var players = LeagueDbService.GetLeaguePlayers(teams);

                if (!Bypass.isSuperUser(ctx.User.Id.ToString()))
                {
                    throw new Exception(Resources.admin_flushServer_unauthorized);
                }


                RankingDbService.DeleteRankings(serverDiscordId);
                ScoreDbService.DeleteGamesFromServer(serverDiscordId);
                LeagueDbService.DeleteTeamPlayers(players);
                LeagueDbService.DeleteTeams(teams);
                LeagueDbService.DeleteLeagues(leagues);
                ServerDbService.DeleteUsersFromServer(serverDiscordId);
                ServerDbService.DeleteServer(serverDiscordId);
                if (server.LeaderboardConfigId != null)
                {
                    ConfigDbService.DeleteConfig((int)server.LeaderboardConfigId);
                }
                if (server.LeaderboardConfigId != null)
                {
                    ConfigDbService.DeleteConfig((int)server.LeaderboardConfigId);
                }

                var rb = new DiscordInteractionResponseBuilder().WithContent(Resources.admin_flushServer_leaderboardEnded);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeConfig", Resources.admin_showConfig_description)]
        public async Task ShowConfig(InteractionContext ctx)
        {
            try
            {
                var configStr = GetConfig(ctx);
                var rb = new DiscordInteractionResponseBuilder().WithContent(configStr).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string GetConfig(InteractionContext ctx)
        {
            var serverDiscordId = ctx.Guild.Id.ToString();
            var server = ServerDbService.GetServer(serverDiscordId);
            int configId;
            if (server.LeaderboardConfigId == null)
            {
                throw new Exception(Resources.commandError_leaderboardNotInitialized);
            }
            configId = (int)server.LeaderboardConfigId;
            
            var config = ConfigDbService.GetConfig(configId);
            var sb = new StringBuilder();
            
            var mod = "**";
            sb.AppendLine($"{mod}startTime: {config.StartTime.ToString("yyyy/MM/dd")}{mod}");
            sb.AppendLine($"{mod}endTime: {(config.EndTime.ToString("yyyy/MM/dd"))}{mod}");
            sb.AppendLine($"{mod}countPoints: {(config.CountPoints ? "Yes" : "No")}{mod}");
            sb.AppendLine($"{mod}allowSanma: {(config.AllowSanma ? "Yes" : "No")}{mod}");
            sb.AppendLine($"{mod}EloSystem: {config.EloSystem}{mod}");
            mod = config.CountPoints ? "**" : "*";
            sb.AppendLine($"{mod}startingPoints: {config.StartingPoints}{mod}");
            mod = config.AllowSanma ? "**" : "*";
            sb.AppendLine($"{mod}uma3p1: {config.Uma3p1}{mod}");
            sb.AppendLine($"{mod}uma3p2: {config.Uma3p2}{mod}");
            sb.AppendLine($"{mod}uma3p3: {config.Uma3p3}{mod}");
            mod = "**";
            sb.AppendLine($"{mod}uma4p1: {config.Uma4p1}{mod}");
            sb.AppendLine($"{mod}uma4p2: {config.Uma4p2}{mod}");
            sb.AppendLine($"{mod}uma4p3: {config.Uma4p3}{mod}");
            sb.AppendLine($"{mod}uma4p4: {config.Uma4p4}{mod}");
            sb.AppendLine($"{mod}oka: {config.Oka}{mod}");
            sb.AppendLine($"{mod}penaltyLast: {config.PenaltyLast}{mod}");
            sb.AppendLine($"{mod}chomboPenalty: {config.PenaltyChombo}{mod}");
            mod = (config.EloSystem != "None" && config.EloSystem != "Average") ? "**" : "*";
            sb.AppendLine($"{mod}baseEloChangeDampening: {config.BaseEloChangeDampening}{mod}");
            mod = config.EloSystem == "Full" ? "**" : "*";
            sb.AppendLine($"{mod}initialElo: {config.InitialElo}{mod}");
            sb.AppendLine($"{mod}minElo: {config.MinElo}{mod}");
            sb.AppendLine($"{mod}eloChangeStartRatio: {config.EloChangeStartRatio}{mod}");
            sb.AppendLine($"{mod}eloChangeEndRatio: {config.EloChangeEndRatio}{mod}");
            sb.AppendLine($"{mod}trialPeriodDuration: {config.TrialPeriodDuration}{mod}");
            return sb.ToString();
        }

        [SlashCommand("setConfig", Resources.admin_setLeaderboardConfig_description)]
        public async Task SetLeaderboardConfig(InteractionContext ctx,
            [Option(Resources.admin_setLeaderboardConfig_countPoints, Resources.admin_setLeaderboardConfig_countPoints_description)] bool countPoints,
            [Choice(Resources.admin_setLeaderboardConfig_eloSystem_Average,"Average")]
            [Choice(Resources.admin_setLeaderboardConfig_eloSystem_None,"None")]
            [Choice(Resources.admin_setLeaderboardConfig_eloSystem_Simple,"Simple")]
            [Choice(Resources.admin_setLeaderboardConfig_eloSystem_Full,"Full")]
            [Option(Resources.admin_setLeaderboardConfig_eloSystem, Resources.admin_setLeaderboardConfig_eloSystem_description)] string eloSystem,
            [Option(Resources.admin_setLeaderboardConfig_startTime, Resources.admin_setLeaderboardConfig_startTime_description)] string startTime = "",
            [Option(Resources.admin_setLeaderboardConfig_endTime, Resources.admin_setLeaderboardConfig_endTime_description)] string endTime = "",
            [Option(Resources.admin_setLeaderboardConfig_startingPoints, Resources.admin_setLeaderboardConfig_startingPoints_description)] long startingPoints = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma3p1, Resources.admin_setLeaderboardConfig_uma3p1_description)] double uma3p1 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma3p2, Resources.admin_setLeaderboardConfig_uma3p2_description)] double uma3p2 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma3p3, Resources.admin_setLeaderboardConfig_uma3p3_description)] double uma3p3 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma4p1, Resources.admin_setLeaderboardConfig_uma4p1_description)] double uma4p1 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma4p2, Resources.admin_setLeaderboardConfig_uma4p2_description)] double uma4p2 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma4p3, Resources.admin_setLeaderboardConfig_uma4p3_description)] double uma4p3 = -1,
            [Option(Resources.admin_setLeaderboardConfig_uma4p4, Resources.admin_setLeaderboardConfig_uma4p4_description)] double uma4p4 = -1,
            [Option(Resources.admin_setLeaderboardConfig_oka, Resources.admin_setLeaderboardConfig_oka_description)] double oka = -1,
            [Option(Resources.admin_setLeaderboardConfig_penaltyLast, Resources.admin_setLeaderboardConfig_penaltyLast_description)] double penaltyLast = -1,
            [Option(Resources.admin_setLeaderboardConfig_penaltyChombo, Resources.admin_setLeaderboardConfig_penaltyChombo_description)] double penaltyChombo = -1,
            [Option(Resources.admin_setLeaderboardConfig_initialElo, Resources.admin_setLeaderboardConfig_initialElo_description)] long initialElo = -1,
            [Option(Resources.admin_setLeaderboardConfig_minElo, Resources.admin_setLeaderboardConfig_minElo_description)] long minElo = -1,
            [Option(Resources.admin_setLeaderboardConfig_eloChangeDampening, Resources.admin_setLeaderboardConfig_eloChangeDampening_description)] double eloChangeDampening = -1,
            [Option(Resources.admin_setLeaderboardConfig_eloChangeStartRatio, Resources.admin_setLeaderboardConfig_eloChangeStartRatio_description)] double eloChangeStartRatio = -1,
            [Option(Resources.admin_setLeaderboardConfig_eloChangeEndRatio, Resources.admin_setLeaderboardConfig_eloChangeEndRatio_description)] double eloChangeEndRatio = -1,
            [Option(Resources.admin_setLeaderboardConfig_trialPeriodDuration, Resources.admin_setLeaderboardConfig_trialPeriodDuration_description)] long trialPeriodDuration = -1)
        {
            var guid = Guid.NewGuid();
            int configId;
            var serverDiscordId = ctx.Guild.Id.ToString();
            try
            {
                var server = ServerDbService.GetServer(serverDiscordId);
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                configId = (int)(server.LeaderboardConfigId);
                var config = ConfigDbService.GetConfig(configId);

                var transactionName = $"setLeaderboardConfig/{guid}";
                DbService.Begin(transactionName);
                try
                {
                    if (startTime.Length > 0)
                    {
                        var startDateTime = DateTime.ParseExact(startTime, "yyyy/MM/dd",
                                           System.Globalization.CultureInfo.InvariantCulture);
                        ConfigDbService.SetConfigValue(ConfigDbService.startDateCol, configId, startDateTime);
                    }
                    if (endTime.Length > 0)
                    {
                        var endDateTime = DateTime.ParseExact(endTime, "yyyy/MM/dd",
                                           System.Globalization.CultureInfo.InvariantCulture);
                        ConfigDbService.SetConfigValue(ConfigDbService.endDateCol, configId, endDateTime);
                    }
                    //hardcoding this for now since it's gonna be pretty much useless until the bot becomes worldwide famous
                    var allowSanma = false;
                    if (allowSanma)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.allowSanmaCol, configId, true);
                        if (uma3p1 != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.uma3p1Col, configId, uma3p1);
                        }
                        if (uma3p2 != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.uma3p2Col, configId, uma3p2);
                        }
                        if (uma3p3 != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.uma3p3Col, configId, uma3p3);
                        }
                    }

                    if (uma4p1 != -1)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.uma4p1Col, configId, uma4p1);
                    }
                    if (uma4p2 != -1)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.uma4p2Col, configId, uma4p2);
                    }
                    if (uma4p3 != -1)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.uma4p3Col, configId, uma4p3);
                    }
                    if (uma4p3 != -1)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.uma4p4Col, configId, uma4p4);
                    }

                    if (countPoints)
                    {
                        ConfigDbService.SetConfigValue(ConfigDbService.countPointsCol, configId, true);
                        if( startingPoints != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.startingPointsCol, configId, startingPoints);
                        }

                        if (oka != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.okaCol, configId, oka);
                        }
                        if (penaltyLast != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.penaltyLastCol, configId, penaltyLast);
                        }
                        if (penaltyChombo != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.penaltyChomboCol, configId, penaltyChombo);
                        }
                    
                    }
                    ConfigDbService.SetConfigValue(ConfigDbService.EloSystemCol, configId, eloSystem);

                    if (eloSystem == "Full" || eloSystem == "Simple")
                    {
                        if (initialElo != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.initialEloCol, configId, initialElo);
                        }
                        if (eloChangeDampening != -1)
                        {
                            ConfigDbService.SetConfigValue(ConfigDbService.baseEloChangeDampeningCol, configId, eloChangeDampening);
                        }
                        if (eloSystem == "Full")
                        {
                            if (minElo != -1)
                            {
                                ConfigDbService.SetConfigValue(ConfigDbService.minEloCol, configId, minElo);
                            }
                            if (eloChangeStartRatio != -1)
                            {
                                ConfigDbService.SetConfigValue(ConfigDbService.eloChangeStartRatioCol, configId, eloChangeStartRatio);
                            }
                            if (eloChangeEndRatio != -1)
                            {
                                ConfigDbService.SetConfigValue(ConfigDbService.eloChangeEndRatioCol, configId, eloChangeEndRatio);
                            }
                            if (trialPeriodDuration != -1)
                            {
                                ConfigDbService.SetConfigValue(ConfigDbService.trialPeriodDurationCol, configId, trialPeriodDuration);
                            }
                        }
                    }
                    var configStr1 = GetConfig(ctx);
                    var rb = new DiscordInteractionResponseBuilder().WithContent(String.Format(Resources.admin_setLeaderboardConfig_backfillInProgress, configStr1)).AsEphemeral();
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);

                }
                catch (Exception e)
                {
                    DbService.Rollback(transactionName);
                    throw e;
                }

                DbService.Commit(transactionName);

                var configStr = GetConfig(ctx);
                StringBuilder sb = new StringBuilder();
                var response = await ctx.GetOriginalResponseAsync();

                var wb = new DiscordWebhookBuilder().WithContent(String.Format(Resources.admin_setLeaderboardConfig_backfillFinished, configStr));
                RankingDbService.BackfillRankings(serverDiscordId, config);
                await ctx.EditResponseAsync(wb);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("addUser", Resources.admin_addPlayer_description)]
        public async Task AddUser(InteractionContext ctx,
            [Option(Resources.admin_addPlayer_nickname, Resources.admin_addPlayer_nickname_description)] string name,
            [Option(Resources.leaderboard_register_mahjsoulName, Resources.leaderboard_register_mahjsoulName_description)] string mahjsoulName = "",
            [Option(Resources.leaderboard_register_tenhouName, Resources.leaderboard_register_tenhouName_description)] string tenhouName = "",
            [Option(Resources.leaderboard_register_riichiCityId, Resources.leaderboard_register_riichiCityId_description)] long riichiCityId = -1,
            [Option(Resources.leaderboard_register_riichiCityName, Resources.leaderboard_register_riichiCityName_description)] string riichiCityName = ""
            )
        {
            try
            {
                var userId = getIdFromPlayerParam(name);
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverDiscordId];
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var configId = (int)(server.LeaderboardConfigId);
                var config = ConfigDbService.GetConfig(configId);

                if (server.Users.Select(x => x.Id).Contains(userId))
                {
                    var nbGames = RankingDbService.GetUserRankingHistory(userId, server.Id).Where(x=>x.GameId>0 && x.OldRank!=null).Count();
                    throw (new Exception(String.Format(Resources.commandError_UserNicknameAlreadyExists,nbGames)));
                }

                if (!users.Keys.Contains(userId))
                {
                    UserDbService.CreateUser(userId, name, serverDiscordId, config);
                }
                if (!server.Users.Select(x => x.Id).Contains(userId))
                {
                    ServerDbService.AddUserToServer(userId, server.Id, userId);
                }

                RankingDbService.InitUserRanking(userId, serverDiscordId, config);
                

                if (mahjsoulName.Length > 0)
                {
                    UserDbService.SetMahjsoulName(userId, mahjsoulName);
                }
                if (tenhouName.Length > 0)
                {
                    UserDbService.SetTenhouName(userId, tenhouName);
                }
                if (riichiCityId >= 0)
                {
                    UserDbService.SetRiichiCityId(userId, (int)riichiCityId);
                }
                if (riichiCityName.Length > 0)
                {
                    UserDbService.SetRiichiCityName(userId, riichiCityName);
                }

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_addPlayer_Success, name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("setUser", Resources.admin_setUser_description)]
        public async Task SetUser(InteractionContext ctx,
            [Option(Resources.admin_addPlayer_nickname, Resources.admin_addPlayer_nickname_description)] string name,
            [Option(Resources.leaderboard_register_mahjsoulName, Resources.leaderboard_register_mahjsoulName_description)] string mahjsoulName = "",
            [Option(Resources.leaderboard_register_tenhouName, Resources.leaderboard_register_tenhouName_description)] string tenhouName = "",
            [Option(Resources.leaderboard_register_riichiCityId, Resources.leaderboard_register_riichiCityId_description)] long riichiCityId = -1,
            [Option(Resources.leaderboard_register_riichiCityName, Resources.leaderboard_register_riichiCityName_description)] string riichiCityName = ""
            )
        {
            try
            {
                var userId = getIdFromPlayerParam(name);
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverDiscordId];
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }

                if (!server.Users.Select(x => x.Id).Contains(userId))
                {
                    throw new Exception(String.Format(Resources.commandError_PlayerUnknown, name));
                }

                if (mahjsoulName.Length > 0)
                {
                    UserDbService.SetMahjsoulName(userId, mahjsoulName);
                }
                if (tenhouName.Length > 0)
                {
                    UserDbService.SetTenhouName(userId, tenhouName);
                }
                if (riichiCityId >= 0)
                {
                    UserDbService.SetRiichiCityId(userId, (int)riichiCityId);
                }
                if (riichiCityName.Length > 0)
                {
                    UserDbService.SetRiichiCityName(userId, riichiCityName);
                }

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_setUser_Success, name)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private string getIdFromPlayerParam(string playerStr)
        {
            if (playerStr.StartsWith("<@") && playerStr.EndsWith(">"))
            {
                return playerStr.Substring(2, playerStr.Length - 3);
            }
            return playerStr;
        }

        [SlashCommand("migrateUser", Resources.admin_migratePlayer_description)]
        public async Task MigratePlayer(InteractionContext ctx,
            [Option(Resources.admin_migratePlayer_sourceName, Resources.admin_migratePlayer_sourceName_description)] string sourceName,
            [Option(Resources.admin_migratePlayer_targetName, Resources.admin_migratePlayer_targetName_description)] string targetName
            )
        {
            var transactionName = $"migration:{sourceName}-{targetName}";
            DbService.Begin(transactionName);
            try
            {

                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var server = servers[serverId]; 
                if (server.LeaderboardConfigId == null)
                {
                    throw new Exception(Resources.commandError_leaderboardNotInitialized);
                }
                var configId = (int)(server.LeaderboardConfigId);
                var config = ConfigDbService.GetConfig(configId);
                var userId = getIdFromPlayerParam(sourceName);
                var targetuserId = getIdFromPlayerParam(targetName);


                //Check if user exists
                if (!server.Users.Select(x => x.Id).Contains(userId))
                {
                    throw (new Exception(Resources.commandError_PlayerUnknown));
                }
                //Create new user in server/db if he's not here already
                if (!users.Keys.Contains(targetuserId))
                {
                    UserDbService.CreateUser(targetuserId, targetName, serverId, config);
                }
                else if (!server.Users.Select(x => x.Id).Contains(targetuserId))
                {
                    ServerDbService.AddUserToServer(targetuserId, serverId, targetName);
                }

                //Change rankings reference to user
                RankingDbService.ChangeUserNameInRankings(userId, targetuserId, serverId);

                //Change games reference to user
                ScoreDbService.ChangeUserNameInGames(userId, targetuserId, serverId);

                //Remove old user from server
                ServerDbService.RemoveUserFromServer(userId,serverId);

                //Update the rankings
                RankingDbService.BackfillRankings(serverId, config);


                DbService.Commit(transactionName);

                var rb = new DiscordInteractionResponseBuilder().WithContent(string.Format(Resources.admin_migratePlayer_success, sourceName, targetuserId)).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                DbService.Rollback(transactionName);
                replyWithException(ctx, e);
            }
        }
        enum CfgPrm
        {
            server,
            name,
            description,
            allowSanma,
            countPoints,
            startingPoints,
            uma3p1,
            uma3p2,
            uma3p3,
            uma4p1,
            uma4p2,
            uma4p3,
            uma4p4,
            oka,
            penaltyLast,
            useEloSystem,
            initialElo,
            minElo,
            baseEloChangeDampening,
            eloChangeStartRatio,
            eloChangeEndRatio,
            trialPeriodDuration,
            UNKNOWN
        }
    }
}
