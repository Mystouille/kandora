using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using kandora.bot.utils;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace kandora.bot.services
{
    class ConfigDbService: DbService
    {
        public static string tableName = "LeaderboardConfig";
        public static string idCol = "Id";
        public static string countPointsCol = "countPoints";
        public static string startingPointsCol = "startingPoints";
        public static string allowSanmaCol = "allowSanma";
        public static string uma3p1Col = "uma3p1";
        public static string uma3p2Col = "uma3p2";
        public static string uma3p3Col = "uma3p3";
        public static string uma4p1Col = "uma4p1";
        public static string uma4p2Col = "uma4p2";
        public static string uma4p3Col = "uma4p3";
        public static string uma4p4Col = "uma4p4";
        public static string okaCol = "oka";
        public static string penaltyLastCol = "penaltyLast";
        public static string penaltyChomboCol = "penaltyChombo";
        public static string EloSystemCol = "EloSystem";
        public static string initialEloCol = "initialElo";
        public static string minEloCol = "minElo";
        public static string baseEloChangeDampeningCol = "baseEloChangeDampening";
        public static string eloChangeStartRatioCol = "eloChangeStartRatio";
        public static string eloChangeEndRatioCol = "eloChangeEndRatio";
        public static string trialPeriodDurationCol = "trialPeriodDuration";
        public static string startDateCol = "startDate";
        public static string endDateCol = "endDate";

        public static LeaderboardConfig GetConfig(int leaderboardConfigId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"SELECT {idCol}, {countPointsCol}, {startingPointsCol}, " //0-2
                    + $"{uma3p1Col}, {uma3p2Col}, {uma3p3Col}, {uma4p1Col}, {uma4p2Col}, {uma4p3Col}, {uma4p4Col}, " // 3-9
                    + $"{okaCol}, {penaltyLastCol}, {EloSystemCol}, {initialEloCol}, {minEloCol}, {baseEloChangeDampeningCol}, " //10-15
                    + $"{eloChangeStartRatioCol}, {eloChangeEndRatioCol}, {trialPeriodDurationCol}, {allowSanmaCol}, " //16-19
                    + $"{startDateCol}, {endDateCol}, {penaltyChomboCol} " // 20-22
                    + $"FROM {tableName} "
                    + $"WHERE {idCol} = @configId";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@configId", NpgsqlDbType.Integer, leaderboardConfigId);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var countPoints = reader.GetBoolean(1);
                    var eloSystem = reader.GetString(12);
                    var leaderboard = new LeaderboardConfig(id, countPoints, eloSystem); ;

                    leaderboard.StartingPoints = reader.GetDouble(2);

                    leaderboard.Uma3p1 = reader.IsDBNull(3) ? -9999 : reader.GetDouble(3);
                    leaderboard.Uma3p2 = reader.IsDBNull(4) ? -9999 : reader.GetDouble(4);
                    leaderboard.Uma3p3 = reader.IsDBNull(5) ? -9999 : reader.GetDouble(5);
                    leaderboard.Uma4p1 = reader.IsDBNull(6) ? -9999 : reader.GetDouble(6);
                    leaderboard.Uma4p2 = reader.IsDBNull(7) ? -9999 : reader.GetDouble(7);
                    leaderboard.Uma4p3 = reader.IsDBNull(8) ? -9999 : reader.GetDouble(8);
                    leaderboard.Uma4p4 = reader.IsDBNull(9) ? -9999 : reader.GetDouble(9);

                    leaderboard.Oka = reader.IsDBNull(10) ? -9999 : reader.GetDouble(10);
                    leaderboard.PenaltyLast = reader.IsDBNull(11) ? -9999 : reader.GetDouble(11);
                    leaderboard.InitialElo = reader.IsDBNull(13) ? -9999 : reader.GetDouble(13);
                    leaderboard.MinElo = reader.IsDBNull(14) ? -9999 : reader.GetDouble(14);
                    leaderboard.BaseEloChangeDampening = reader.IsDBNull(15) ? -9999 : reader.GetDouble(15);

                    leaderboard.EloChangeStartRatio = reader.IsDBNull(16) ? -9999 : reader.GetDouble(16);
                    leaderboard.EloChangeEndRatio = reader.IsDBNull(17) ? -9999 : reader.GetDouble(17);
                    leaderboard.TrialPeriodDuration = reader.IsDBNull(18) ? -9999 : reader.GetInt32(18);
                    leaderboard.AllowSanma = reader.GetBoolean(19);

                    DateTime startDate = reader.GetDateTime(20);
                    DateTime endDate = reader.GetDateTime(21);
                    leaderboard.StartTime = reader.GetDateTime(20);
                    leaderboard.EndTime = reader.GetDateTime(21);
                    leaderboard.PenaltyChombo = reader.IsDBNull(22) ? -9999 : reader.GetDouble(22);

                    reader.Close();
                    return leaderboard;
                }
                reader.Close();
                throw new Exception($"Couldn't find config with id {leaderboardConfigId}");
            }
            throw (new DbConnectionException());
        }

        public static int CreateConfig()
        {

            var settings = ConfigurationManager.AppSettings;
            var countPoints = bool.Parse(settings.Get("countPoints"));
            var eloSystem = settings.Get("eloSystem");
            var startingPoints = Double.Parse(settings.Get("startingPoints"));
            var allowSanma = Boolean.Parse(settings.Get("allowSanma"));
            var uma3p1 = Double.Parse(settings.Get("uma3p1"));
            var uma3p2 = Double.Parse(settings.Get("uma3p2"));
            var uma3p3 = Double.Parse(settings.Get("uma3p3"));
            var uma4p1 = Double.Parse(settings.Get("uma4p1"));
            var uma4p2 = Double.Parse(settings.Get("uma4p2"));
            var uma4p3 = Double.Parse(settings.Get("uma4p3"));
            var uma4p4 = Double.Parse(settings.Get("uma4p4"));
            var oka = Double.Parse(settings.Get("oka"));
            var penaltyLast = Double.Parse(settings.Get("penaltyLast"));
            var penaltyChombo = Double.Parse(settings.Get("penaltyChombo"));
            var initialElo = Double.Parse(settings.Get("initialElo"));
            var minElo = Double.Parse(settings.Get("minElo"));
            var baseEloChangeDampening = Double.Parse(settings.Get("baseEloChangeDampening"));
            var eloChangeStartRatio = Double.Parse(settings.Get("eloChangeStartRatio"));
            var eloChangeEndRatio = Double.Parse(settings.Get("eloChangeEndRatio"));
            var trialPeriodDuration = Double.Parse(settings.Get("trialPeriodDuration"));

            var configId = CreateConfig(countPoints, eloSystem);
            SetConfigValue(startingPointsCol, configId, startingPoints);
            SetConfigValue(allowSanmaCol, configId, allowSanma);
            SetConfigValue(uma3p1Col, configId, uma3p1);
            SetConfigValue(uma3p2Col, configId, uma3p2);
            SetConfigValue(uma3p3Col, configId, uma3p3);
            SetConfigValue(uma4p1Col, configId, uma4p1);
            SetConfigValue(uma4p2Col, configId, uma4p2);
            SetConfigValue(uma4p3Col, configId, uma4p3);
            SetConfigValue(uma4p4Col, configId, uma4p4);
            SetConfigValue(okaCol, configId, oka);
            SetConfigValue(penaltyLastCol, configId, penaltyLast);
            SetConfigValue(penaltyChomboCol, configId, penaltyChombo);
            SetConfigValue(initialEloCol, configId, initialElo);
            SetConfigValue(minEloCol, configId, minElo);
            SetConfigValue(baseEloChangeDampeningCol, configId, baseEloChangeDampening);
            SetConfigValue(eloChangeStartRatioCol, configId, eloChangeStartRatio);
            SetConfigValue(eloChangeEndRatioCol, configId, eloChangeEndRatio);
            SetConfigValue(trialPeriodDurationCol, configId, trialPeriodDuration);

            return configId;
        }

        public static void DeleteConfig(int leaderboardConfigId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"DELETE FROM {tableName} WHERE {idCol} = {leaderboardConfigId};";
                command.CommandType = CommandType.Text;
                var reader = command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        private static int CreateConfig(bool countPoints, string eloSystem)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"INSERT INTO {tableName} ({countPointsCol}, {EloSystemCol}) "
                + $"VALUES (@countPoints, @EloSystem) "
                + $"RETURNING {idCol} ";

                command.Parameters.AddWithValue("@countPoints", NpgsqlDbType.Boolean, countPoints);
                command.Parameters.AddWithValue("@EloSystem", NpgsqlDbType.Varchar, eloSystem);
                command.CommandType = CommandType.Text;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var configId = reader.GetInt32(0);
                    reader.Close();
                    return configId;
                }
                throw new Exception("couldn't retrieve the new config id");

            }
            throw (new DbConnectionException());
        }

        public static void SetConfigValue<T1,T2>(string configParamName, T1 configId, T2 configParamValue)
        {
            UpdateFieldInTable(tableName, configParamName, configId, configParamValue);
        }
    }
}
