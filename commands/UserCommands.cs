using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using kandora.bot.exceptions;
using kandora.bot.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.commands
{
    public class UserCommands : KandoraCommandModule
    {
        [Command("activateLeague"), Description("register the current server as a league host and creat the KandoraLeague role")]
        public async Task RegisterServer(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterServerAction(ctx),
                serverMustBeRegistered: false,
                userMustBeInChannel: false,
                userMustBeRegistered: false
            );
        }
        private Func<Task> getRegisterServerAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var leagueConfigId = LeagueConfigDbService.CreateLeague();
                var roleName = "KandoraLeague";
                ulong roleId = ctx.Guild.Roles.Where(x => x.Value.Name == roleName).Select(x => x.Key).FirstOrDefault();
                if (roleId == 0)
                {
                    var role = await ctx.Guild.CreateRoleAsync(name: roleName, mentionable: true);
                    roleId = role.Id;
                }
                ServerDbService.AddServer(serverDiscordId, ctx.Guild.Name, roleId.ToString(), roleName, leagueConfigId);
                await ctx.RespondAsync($"A Riichi league has started on {ctx.Guild.Name}!! \n");
            });
        }

        [Command("setTargetChannel"), Description("Set bot to listen to the current channel")]
        public async Task SetTargetChannel(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getSetTargetChannelAction(ctx),
                userMustBeInChannel: false,
                userMustBeRegistered: false
            );
        }

        private Func<Task> getSetTargetChannelAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var serverDiscordId = ctx.Guild.Id.ToString();
                ServerDbService.SetTargetChannel(serverDiscordId, ctx.Channel.Id.ToString());
                await ctx.RespondAsync($"<#{ctx.Channel.Id}> has been registered as scoring channel");
            });
        }

        [Command("register"), Description("Register yourself in the local riichi league")]
        public async Task RegisterUser(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterUserAction(ctx),
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterUserAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                UserDbService.CreateUser(discordId, serverDiscordId, config);
                ServerDbService.AddUserToServer(discordId, serverDiscordId, false, false);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId)) {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.GrantRoleAsync(ctx.Guild.Roles[roleId], "registering for riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been registered");
            });
        }

        [Command("registerdummy"), Description("Register dummy people")]
        public async Task RegisterDummy(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getRegisterDummyAction(ctx),
                userMustBeRegistered: false
            );
        }

        private Func<Task> getRegisterDummyAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var server = ServerDbService.GetServer(serverDiscordId);
                var config = LeagueConfigDbService.GetLeagueConfig(server.LeagueConfigId);
                var heatiro = "323096688904634377";
                var clubapero = "198974501709414401";
                var Neral = "273192430172372993";
                UserDbService.CreateUser(heatiro, serverDiscordId, config);
                UserDbService.CreateUser(clubapero, serverDiscordId, config);
                UserDbService.CreateUser(Neral, serverDiscordId, config);
                ServerDbService.AddUserToServer(heatiro, serverDiscordId, false, false); //Heatiro
                ServerDbService.AddUserToServer(clubapero, serverDiscordId, false, false); //clubapero
                ServerDbService.AddUserToServer(Neral, serverDiscordId, false, false); //Neral
                UserDbService.SetMahjsoulName(heatiro, "heairo");
                UserDbService.SetMahjsoulName(Neral, "Neral");
                UserDbService.SetMahjsoulName(clubapero, "clubapero");
            });
        }

        [Command("unregister"), Description("Remove yourself in the kandora riichi league")]
        public async Task UnRegisterUser(CommandContext ctx)
        {
            await executeCommand(
                ctx,
                getUnRegisterUserAction(ctx),
                userMustBeRegistered: true
            );
        }
        private Func<Task> getUnRegisterUserAction(CommandContext ctx)
        {
            return new Func<Task>(async () =>
            {
                var displayName = ctx.User.Username;
                var discordId = ctx.User.Id.ToString();
                var serverDiscordId = ctx.Guild.Id.ToString();
                var users = UserDbService.GetUsers();
                var servers = ServerDbService.GetServers(users);
                if (!servers.ContainsKey(serverDiscordId))
                {
                    throw new UserNotRegisteredException();
                }
                var server = servers[serverDiscordId];
                ServerDbService.RemoveUserFromServer(discordId, serverDiscordId);
                ulong roleId = Convert.ToUInt64(server.LeagueRoleId);
                if (!ctx.Guild.Roles.ContainsKey(roleId))
                {
                    throw new Exception("Error: League role not found");
                }
                await ctx.Member.RevokeRoleAsync(ctx.Guild.Roles[roleId], "removing from riichi league");
                await ctx.RespondAsync($"<@{ctx.User.Id}> has been removed from league");
            });
        }

        private enum UserProperty
        {
            MahjsoulName,
            MahjsoulFriendId,
            TenhouName,
            Unknown,
        }


        [Command("displayMe"), Description("Display your info")]
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
                sb.AppendLine($"Leagues you are registered in:");
                foreach(var server in servers)
                {
                    var rankings = RankingDbService.GetServerRankings(server.Id);
                    var playersRank = new Dictionary<string, float>();
                    var playersTime = new Dictionary<string, DateTime>();
                    //Get the last ranking for each player by iterating through the whole list
                    foreach(var ranking in rankings)
                    {
                        var playerId = ranking.UserId;
                        float rank;
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
                    List<KeyValuePair<string, float>> flatRanksSorted = playersRank.ToList();
                    flatRanksSorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    var nbPlayers = server.Users.Count;
                    var userRank = 1;
                    float userElo = -1;
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
                await ctx.RespondAsync(sb.ToString());
            });
        }

        [Command("changeme"), Description("Change your info")]
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
                    default: throw new Exception($"You can't change your {propertyStr}");
                }
                await ctx.RespondAsync($"<@{ctx.User.Id}>'s {property} has been changed to {value}.");
            });
        }

        [Command("listusers"), Description("List the users in Kandora league"), Aliases("l")]
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
                sb.Append("User list:\n");
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