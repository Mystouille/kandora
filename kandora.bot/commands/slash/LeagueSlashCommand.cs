using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kandora.bot.http;
using kandora.bot.models;
using kandora.bot.resources;
using kandora.bot.services;
using kandora.bot.services.discord;
using kandora.bot.services.http;
using kandora.bot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.slash
{
    [SlashCommandGroup("league", Resources.admin_groupDescription, defaultPermission: false)]
    class LeagueSlashCommands : KandoraSlashCommandModule
    {
        [SlashCommand("getLogInfo", Resources.league_logInfo_description)]
        public async Task GetLogInfo(InteractionContext ctx,
             [Option(Resources.league_option_gameId, Resources.league_option_gameId_description)] string gameId)
        {
            try
            {
                var log = await LogService.Instance.GetLog(gameId, 2);
                var gameResult = PrintGameResult(log, ctx.Client);

                var rb = new DiscordInteractionResponseBuilder().WithContent(gameResult).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("submitResult", Resources.league_submitResult_description)]
        public async Task SubmitResult(InteractionContext ctx,
            [Option(Resources.league_option_player1, Resources.league_option_anyPlayer_description)] DiscordUser player1,
            [Option(Resources.league_option_player1Score, Resources.league_option_anyScore_description)] string score1,
            [Option(Resources.league_option_player2, Resources.league_option_anyPlayer_description)] DiscordUser player2,
            [Option(Resources.league_option_player2Score, Resources.league_option_anyScore_description)] string score2,
            [Option(Resources.league_option_player3, Resources.league_option_anyPlayer_description)] DiscordUser player3,
            [Option(Resources.league_option_player3Score, Resources.league_option_anyScore_description)] string score3,
            [Option(Resources.league_option_player4, Resources.league_option_anyPlayer_description)] DiscordUser player4,
            [Option(Resources.league_option_player4Score, Resources.league_option_anyScore_description)] string score4)
        {
            try
            {
                var usersScores = new (DiscordUser, string)[] { (player1, score1), (player2, score2), (player3, score3), (player4, score4) };
                var sortedScores = usersScores.ToList();
                sortedScores.Sort((tuple1, tuple2) => tuple2.Item2.CompareTo(tuple1.Item2));

                var usersIds = sortedScores.Select(x => x.Item1.Id.ToString()).ToArray();
                var scores = sortedScores.Select(x => x.Item2).ToArray();
                await scoreIrlMatch(ctx, usersIds, scores);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        [SlashCommand("seeRanking", Resources.league_seeRanking_description)]
        public async Task SeeRanking(InteractionContext ctx)
        {
            try
            {
                var userId = ctx.User.Id.ToString();
                var serverId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                List<Ranking> rankingList = RankingDbService.GetServerRankings(serverId);
                var latestUserRankings = new Dictionary<string, Ranking>();
                foreach (var rank in rankingList)
                {
                    var id = rank.UserId;
                    if (!latestUserRankings.ContainsKey(id))
                    {
                        latestUserRankings.Add(rank.UserId, rank);
                    }
                }

                List<Ranking> sortedRanks = latestUserRankings.Values.ToList();
                sortedRanks.Sort((val1, val2) => val2.NewRank.CompareTo(val1.NewRank));

                StringBuilder sb = new StringBuilder();
                int i = 1;
                sb.AppendLine(":partying_face:");
                foreach (var rank in sortedRanks)
                {
                    sb.Append($"{i}: <@{rank.UserId}> ({rank.NewRank}) {(rank.UserId == userId ? $"<<< {Resources.league_seeRanking_youAreHere}" : "")}\n");
                    i++;
                }
                var leaderboard = sb.ToString();
                var rb = new DiscordInteractionResponseBuilder().WithContent(leaderboard).AsEphemeral();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                replyWithException(ctx, e);
            }
        }

        private async Task scoreIrlMatch(InteractionContext ctx, string[] userIds, string[] scoresStr = null)
        {
            float[] scores = null;
            if (scoresStr != null)
            {
                scores = scoresStr.Select(x => float.Parse(x)).ToArray();
            }
            var serverId = ctx.Guild.Id.ToString();
            var userId = ctx.Member.Id.ToString();
            var channelDiscordId = ctx.Channel.Id.ToString();
            var allUsers = UserDbService.GetUsers();
            var servers = ServerDbService.GetServers(allUsers);
            var server = servers[serverId];
            foreach (var id in userIds)
            {
                if (server.Users.Where(x => x.Id == id).Count() == 0)
                {
                    throw new Exception($"{String.Format(Resources.commandError_CouldNotFindGameUser, id)}");
                }
            }
            var leagueConfig = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
            if (leagueConfig.CountPoints && scores == null)
            {
                throw new Exception(Resources.commandError_LeagueConfigRequiresScore);
            }
            if (!leagueConfig.AllowSanma && userIds.Length == 3)
            {
                throw new Exception(Resources.commandError_sanmaNotAllowed);
            }
            if (userIds.Length < 3 || userIds.Length > 4)
            {
                throw new Exception(String.Format(Resources.commandError_badPlayerNumber, userIds.Length));
            }
            var distinctUsers = userIds.Distinct();
            if (distinctUsers.Count() < userIds.Length)
            {
                throw new Exception(String.Format(Resources.commandError_badDistinctPlayerNumber, distinctUsers.Count()));
            }

            var gameResult = PrintGameResult(ctx.Client, userIds, scores);

            var gameMsg = $"{Resources.league_submitResult_voteMessage}\n{gameResult}";
            var rb = new DiscordInteractionResponseBuilder().WithContent(gameMsg);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, rb).ConfigureAwait(true);
            var response = await ctx.GetOriginalResponseAsync();
            await kandoraContext.AddPendingGame(ctx, response, new PendingGame(userIds, scores, server));
        }

        private static string PrintGameResult(RiichiGame game, DiscordClient client, List<User> users = null)
        {
            StringBuilder sb = new StringBuilder();
            if (game.Title != null)
            {
                sb.Append($"{Resources.league_title}: {game.Title[0]}\n");
                sb.Append($"{Resources.league_date}: {game.Title[1]}\n");
            }
            sb.Append($"{Resources.league_results}: \n");
            var names = game.Names;
            var discordIds = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                //Also get the discord user's mention
                if (users != null)
                {
                    User user = null;
                    if (game.GameType == GameType.Tenhou)
                    {
                        user = users.Find(x => x.TenhouName == name);
                    }
                    else if (game.GameType == GameType.Mahjsoul)
                    {
                        user = users.Find(x => x.MahjsoulName == name);
                        if (user == null)
                        {
                            user = users.Find(x => x.MahjsoulUserId != null && x.MahjsoulUserId == game.UserIds[i]);
                        }
                    }
                    if (user != null)
                    {
                        discordIds[i] = $"<@{user.Id}>";
                    }
                    else
                    {
                        throw new Exception($"{String.Format(Resources.commandError_CouldNotFindGameUser, game.GameTypeStr, name)}");
                    }
                }
                sb.Append($"{discordIds[i]}({name}):\t{game.FinalScores[i]}\t({game.FinalRankDeltas[i]})\n");
            }
            var bestPayment = 0;
            RoundResult bestResult = null;
            Round bestRound = null;
            var player = -1;
            foreach (var log in game.Rounds)
            {
                foreach (var res in log.Result)
                {
                    var i = 0;
                    foreach (var delta in res.Payments)
                    {
                        if (delta > bestPayment)
                        {
                            bestPayment = delta;
                            bestResult = res;
                            bestRound = log;
                            player = i;
                        }
                        i++;
                    }
                }
            }
            sb.AppendLine($"{String.Format(Resources.league_bestHand,discordIds[player],game.Names[player],bestRound.RoundNumber,bestResult.HandScore,bestPayment)}{getReactionFromScore(client, bestPayment)}");
            return sb.ToString();
        }

        private static string PrintGameResult(DiscordClient client, string[] userIds, float[] scores = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("IRL Game:");
            for (int i = 0; i < userIds.Length; i++)
            {
                sb.AppendLine($"{i + 1}: <@{userIds[i]}>: {scores[i]}");
            }
            return sb.ToString();
        }

        private static string getReactionFromScore(DiscordClient client, int score)
        {
            if(score < 8000)
            {
                return DiscordEmoji.FromName(client, Reactions.WOW);
            }
            return DiscordEmoji.FromName(client, Reactions.WOW);
        }
    }
}
