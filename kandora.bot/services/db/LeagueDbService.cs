using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace kandora.bot.services
{
    class LeagueDbService : DbService
    {
        public static string LeagueTableName = "League";
        public static string teamTableName = "Team";
        public static string teamUserTableName = "TeamUser";

        //id col
        public static string idCol = "Id";

        //League
        public static string displayNameCol = "displayName";
        public static string serverIdCol = "serverId";
        public static string isOngoingCol = "isOngoing";

        //Team
        public static string teamNameCol = "teamName";
        public static string fancyNameCol = "fancyName";
        public static string leagueIdCol = "leagueId";

        //TeamUser
        public static string teamIdCol = "teamId";
        public static string userIdCol = "userId";
        public static string isCaptainCol = "isCaptain";



        public static List<League> GetLeaguesOnServer(string serverId, bool onlyOngoing = false)
        {
            var leagues = new List<League>();
            Dictionary<string,Server> serverMap = new Dictionary<string, Server>();
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {displayNameCol} FROM {LeagueTableName} WHERE {serverIdCol} = @serverId {(onlyOngoing ? $"AND {isOngoingCol} = true" : "")};";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                var reader = command.ExecuteReader();

                League league = null;
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string displayName = reader.GetString(1);
                    league = new League(id, displayName, serverId, true);
                    leagues.Add(league);
                }
                reader.Close();
                return leagues;
            }
            throw new DbConnectionException();
        }

        public static void StartLeague(string serverId, int leagueId, string leagueName)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"INSERT INTO {LeagueTableName} ({idCol}, {serverIdCol}, {isOngoingCol}, {displayNameCol}) VALUES (@configId, @serverId, true, @displayName);";

                command.Parameters.AddWithValue("@configId", NpgsqlDbType.Integer, leagueId);
                command.Parameters.AddWithValue("@serverId", NpgsqlDbType.Varchar, serverId);
                command.Parameters.AddWithValue("@displayName", NpgsqlDbType.Varchar, leagueName);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw new DbConnectionException();
        }

        public static void DeleteLeagues(List<League> leagues)
        {
            if (leagues.Count == 0)
            {
                return;
            }
            var leagueIds = string.Join(",", leagues.Select(x => x.Id));
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"DELETE FROM {LeagueTableName} WHERE {idCol} IN ({leagueIds});";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw new DbConnectionException();
        }

        public static void DeleteTeams(List<Team> teams)
        {
            if (teams.Count == 0)
            {
                return;
            }
            var dbCon = DBConnection.Instance();
            var teamIds = string.Join(",", teams.Select(x => x.teamId));
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"DELETE FROM {teamTableName} WHERE {idCol} IN ({teamIds});";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw new DbConnectionException();
        }

        public static void DeleteTeamPlayers(List<TeamUser> players)
        {
            if(players.Count == 0)
            {
                return;
            }
            var dbCon = DBConnection.Instance();
            var playerIds = string.Join(",", players.Select(x => x.id));
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"DELETE FROM {teamUserTableName} WHERE {idCol} IN ({playerIds});";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw new DbConnectionException();
        }

        public static void CreateLeagueTeam(int leagueId, string teamName, string fancyName)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"INSERT INTO {teamTableName} ({leagueIdCol}, {teamNameCol}, {fancyNameCol}) VALUES (@leagueId, @teamName, @fancyName) RETURNING {idCol};";

                command.Parameters.AddWithValue("@leagueId", NpgsqlDbType.Integer, leagueId);
                command.Parameters.AddWithValue("@teamName", NpgsqlDbType.Varchar, teamName);
                command.Parameters.AddWithValue("@fancyName", NpgsqlDbType.Varchar, fancyName);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();

                //Read one line only
                reader.Read();
                int teamId = reader.GetInt32(0); // might be useful later
                reader.Close();
                return;
            }
            throw new DbConnectionException();
        }

        
        public static List<Team> GetLeagueTeams(List<League> leagues)
        {
            List<Team> teamList = new List<Team>();
            if (leagues.Count == 0)
            {
                return teamList;
            }
            var dbCon = DBConnection.Instance();
            var leagueIds = string.Join(",", leagues.Select(x => x.Id));
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {idCol}, {teamNameCol}, {fancyNameCol} FROM {teamTableName} " +
                    $"WHERE {leagueIdCol} IN ({leagueIds});";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string teamName = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    string fancyName = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    teamList.Add(new Team(id, teamName, fancyName));
                }
                reader.Close();
                return teamList;
            }
            throw new DbConnectionException();
        }
        
        public static List<TeamUser> GetLeaguePlayers(List<Team> teams)
        {
            List<TeamUser> playerList = new List<TeamUser>();
            if(teams.Count == 0)
            {
                return playerList;
            }
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                var allTeams = string.Join(",", teams.Select(x=>x.teamId));
                command.CommandText = $"SELECT {idCol}, {teamIdCol}, {isCaptainCol}, {userIdCol} FROM {teamUserTableName} " +
                    $"WHERE {teamIdCol} IN ({allTeams});";
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int teamId = reader.GetInt32(1);
                    bool isCaptain = reader.GetBoolean(2);
                    string userId = reader.GetString(3);
                    playerList.Add(new TeamUser(id,userId, teamId, isCaptain));
                }
                reader.Close();
                return playerList;
            }
            throw new DbConnectionException();
        }

        public static void AddPlayerToTeam(string userId, int teamId, bool isCaptain)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandType = CommandType.Text;
                command.CommandText = $"INSERT INTO {teamUserTableName} ({teamIdCol}, {userIdCol}, {isCaptainCol}) VALUES (@teamId, @userId, @isCaptain);";

                command.Parameters.AddWithValue("@teamId", NpgsqlDbType.Integer, teamId);
                command.Parameters.AddWithValue("@userId", NpgsqlDbType.Varchar, userId);
                command.Parameters.AddWithValue("@isCaptain", NpgsqlDbType.Boolean, isCaptain);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw new DbConnectionException();
        }

    }
}
