﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kandora.bot.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands.regular
{
    public class UserCommands : KandoraCommandModule
    {
        private enum UserProperty
        {
            MahjsoulName,
            MahjsoulFriendId,
            TenhouName,
            LeaguePassword,
            Unknown,
        }


        [Command("getMe"), Description("Display your info")]
        public async Task DisplayMe(CommandContext ctx)
        {
            await executeMpCommand(
                ctx,
                getDisplayMeAction(ctx),
                userMustBeInMP: false
            );
        }


        private Func<Task> getDisplayMeAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var userId = ctx.User.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                if (!allUsers.ContainsKey(userId))
                {
                    throw new Exception("You are not registered in any server");
                }
                var user = allUsers[userId];
                var allServers = ServerDbService.GetServers(allUsers);
                var servers = allServers.Values.Where(x => x.Users.Contains(user));
                var sb = new StringBuilder();

                sb.AppendLine("Mahjsoul info:");
                sb.AppendLine($"\t{UserProperty.MahjsoulName}: {(user.MahjsoulName == null ? "N/A" : user.MahjsoulName)} (can be changed)");
                sb.AppendLine($"\t{UserProperty.MahjsoulFriendId}: {(user.MahjsoulFriendId == null ? "N/A" : user.MahjsoulFriendId)} (can be changed)");
                sb.AppendLine($"\tMahjsoulInternalUserId: {(user.MahjsoulUserId == null ? "N/A" : user.MahjsoulUserId)}");
                sb.AppendLine($"\tLast known rank: N/A"); // TODO
                sb.AppendLine("Tenhou info:");
                sb.AppendLine($"\t{UserProperty.TenhouName}: {(user.TenhouName == null ? "N/A" : user.TenhouName)} (can be changed)");
                sb.AppendLine($"\tLast known rank: N/A"); // TODO
                sb.AppendLine($"DiscordId: {user.Id}");
                sb.AppendLine($"LeaguePassword: {user.LeaguePassword} (can be changed)");
                sb.AppendLine($"Leagues you are registered in:");
                foreach(var server in servers)
                {
                    var rankings = RankingDbService.GetServerRankings(server.Id);
                    var playersRank = new Dictionary<string, double>();
                    var playersTime = new Dictionary<string, DateTime>();
                    //Get the last ranking for each player by iterating through the whole list
                    foreach(var ranking in rankings)
                    {
                        var playerId = ranking.UserId;
                        double rank;
                        DateTime time;
                        if (!playersRank.TryGetValue(playerId, out rank) || !playersTime.TryGetValue(playerId, out time))
                        {
                            playersRank.Add(playerId, ranking.NewRank);
                            playersTime.Add(playerId, ranking.Timestamp);
                        }
                        else
                        {
                            if (ranking.Timestamp.CompareTo(time)> 0)
                            {
                                playersRank[playerId] = ranking.NewRank;
                                playersTime[playerId] = ranking.Timestamp;
                            }
                        }
                    }

                    //Sort the list and compute the user's rank and ELO
                    List<KeyValuePair<string, double>> flatRanksSorted = playersRank.ToList();
                    flatRanksSorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    var nbPlayers = server.Users.Count;
                    var userRank = 1;
                    double userElo = -1;
                    foreach (var rank in flatRanksSorted)
                    {
                        if( rank.Key == userId)
                        {
                            userElo = rank.Value;
                            break;
                        }
                        else
                        {
                            userRank++;
                        }
                    }
                    sb.AppendLine($"\t{server.LeagueName}({server.DisplayName}): {userRank}/{nbPlayers} (R{userElo})");
                }
                if (ctx != null && ctx.Member == null)
                {
                    await ctx.RespondAsync(sb.ToString());
                }
                else
                {
                    await ctx.Member.SendMessageAsync(sb.ToString());
                }
            });
        }

        [Command("changeMe"), Description("Change your info")]
        public async Task ChangeMe(CommandContext ctx, [Description("What to change")] string property, [Description("The new name/id you want")] string value)
        {
            await executeMpCommand(
                ctx,
                getChangeMeAction(ctx, property, value),
                userMustBeInMP: false
            );
        }


        private Func<Task> getChangeMeAction(CommandContext ctx, string propertyStr, string value)
        {
            return new Func<Task>(async () =>
            {
                var userId = ctx.User.Id.ToString();
                var allUsers = UserDbService.GetUsers();
                if (!allUsers.ContainsKey(userId))
                {
                    throw new Exception("You are not registered in any server");
                }
                UserProperty property = UserProperty.Unknown;
                try
                {
                    property = (UserProperty)Enum.Parse(typeof(UserProperty), propertyStr);
                } catch {
                    throw new Exception("This is not an existing or editable property");
                }
                switch (property)
                {
                    case UserProperty.MahjsoulName: UserDbService.SetMahjsoulName(userId, value); break;
                    case UserProperty.MahjsoulFriendId: UserDbService.SetMahjsoulFriendId(userId, value); break;
                    case UserProperty.TenhouName: UserDbService.SetTenhouName(userId, value); break;
                    case UserProperty.LeaguePassword: UserDbService.SetLeaguePassword(userId, value); break;
                    default: throw new Exception($"You can't change your {propertyStr}");
                }
                await ctx.RespondAsync($"<@{ctx.User.Id}>'s {property} has been changed to {value}.");
            });
        }

        [Command("getList"), Description("List the users in Kandora league"), Aliases("l")]
        public async Task List(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getListAction(ctx),
                userMustBeRegistered: false,
                userMustBeInChannel: true
            );
        }

        private Func<Task> getListAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();

                int i = 1;
                StringBuilder sb = new StringBuilder();
                sb.Append($"Users registered in {ctx.Guild.Name}:\n");
                foreach (var user in servers[serverDiscordId].Users)
                {
                    sb.Append($"<@{user.Id}> majsoulName: {user.MahjsoulName} {(user.Id == discordId ? " <<< you" : "")}\n");
                    i++;
                }
                if (ctx != null && ctx.Member == null)
                {
                    await ctx.RespondAsync(sb.ToString());
                }
                else
                {
                    await ctx.Member.SendMessageAsync(sb.ToString());
                }
            });
        }
    }
}