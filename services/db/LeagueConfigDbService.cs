using kandora.bot.exceptions;
using kandora.bot.models;
using kandora.bot.services.db;
using System;
using System.Data;
using System.Data.SqlClient;

namespace kandora.bot.services
{
    class LeagueConfigDbService: DbService
    {
        public static string tableName = "[dbo].[LeagueConfig]";
        public static string idCol = "Id";
        public static string countPointsCol = "countPoints";
        public static string startingPointsCol = "startingPoints";
        public static string uma3p1Col = "uma3p1";
        public static string uma3p2Col = "uma3p2";
        public static string uma3p3Col = "uma3p3";
        public static string uma4p1Col = "uma4p1";
        public static string uma4p2Col = "uma4p2";
        public static string uma4p3Col = "uma4p3";
        public static string uma4p4Col = "uma4p4";
        public static string okaCol = "oka";
        public static string penaltyLastCol = "penaltyLast";
        public static string useEloSystemCol = "useEloSystem";
        public static string initialEloCol = "initialElo";
        public static string minEloCol = "minElo";
        public static string baseEloChangeDampeningCol = "baseEloChangeDampening";
        public static string eloChangeStartRatioCol = "eloChangeStartRatio";
        public static string eloChangeEndRatioCol = "eloChangeEndRatio";
        public static string trialPeriodDurationCol = "trialPeriodDuration";

        public static LeagueConfig GetLeagueConfig(int leagueConfigId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {idCol}, {countPointsCol}, {startingPointsCol}, " //0-2
                    + $"{uma3p1Col}, {uma3p2Col}, {uma3p3Col}, {uma4p1Col}, {uma4p2Col}, {uma4p3Col}, {uma4p4Col}, " // 3-9
                    + $"{okaCol}, {penaltyLastCol}, {useEloSystemCol}, {initialEloCol}, {minEloCol}, {baseEloChangeDampeningCol}, " //10-15
                    + $"{eloChangeStartRatioCol}, {eloChangeEndRatioCol}, {trialPeriodDurationCol} " //16-18
                    + $"FROM {tableName} "
                    + $"WHERE {idCol} = @leagueId";
                command.CommandType = CommandType.Text;
                command.Parameters.Add(new SqlParameter("@leagueId", SqlDbType.VarChar)
                {
                    Value = leagueConfigId
                });

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var countPoints = reader.GetBoolean(1);
                    var useEloSystem = reader.GetBoolean(12);
                    var league = new LeagueConfig(id, countPoints, useEloSystem); ;

                    league.StartingPoints = (float)reader.GetDouble(2);

                    league.Uma3p1 = reader.IsDBNull(3) ? -9999 : (float)reader.GetDouble(3);
                    league.Uma3p2 = reader.IsDBNull(4) ? -9999 : (float)reader.GetDouble(4);
                    league.Uma3p3 = reader.IsDBNull(5) ? -9999 : (float)reader.GetDouble(5);
                    league.Uma4p1 = reader.IsDBNull(6) ? -9999 : (float)reader.GetDouble(6);
                    league.Uma4p2 = reader.IsDBNull(7) ? -9999 : (float)reader.GetDouble(7);
                    league.Uma4p3 = reader.IsDBNull(8) ? -9999 : (float)reader.GetDouble(8);
                    league.Uma4p4 = reader.IsDBNull(9) ? -9999 : (float)reader.GetDouble(9);

                    league.Oka = reader.IsDBNull(10) ? -9999 : (float)reader.GetDouble(10);
                    league.PenaltyLast = reader.IsDBNull(11) ? -9999 : (float)reader.GetDouble(11);
                    league.InitialElo = reader.IsDBNull(13) ? -9999 : (float)reader.GetDouble(13);
                    league.MinElo = reader.IsDBNull(14) ? -9999 : (float)reader.GetDouble(14);
                    league.BaseEloChangeDampening = reader.IsDBNull(15) ? -9999 : (float)reader.GetDouble(15);

                    league.EloChangeStartRatio = reader.IsDBNull(16) ? -9999 : (float)reader.GetDouble(16);
                    league.EloChangeEndRatio = reader.IsDBNull(17) ? -9999 : (float)reader.GetDouble(17);
                    league.TrialPeriodDuration = reader.IsDBNull(18) ? -9999 : reader.GetInt32(18);

                    reader.Close();
                    return league;
                }
                reader.Close();
                throw new Exception($"Couldn't find league with id {leagueConfigId}");
            }
            throw (new DbConnectionException());
        }

        public static int CreateLeague()
        {
            var leagueId = CreateLeague(countPoints: false, useEloSystem: true);
            SetConfigValue(startingPointsCol, leagueId, 0);
            SetConfigValue(uma3p1Col, leagueId, (float)20);
            SetConfigValue(uma3p2Col, leagueId, (float)0);
            SetConfigValue(uma3p3Col, leagueId, (float)(-20));
            SetConfigValue(uma4p1Col, leagueId, (float)30);
            SetConfigValue(uma4p2Col, leagueId, (float)10);
            SetConfigValue(uma4p3Col, leagueId, (float)(-10));
            SetConfigValue(uma4p4Col, leagueId, (float)(-30));
            SetConfigValue(okaCol, leagueId, (float)0);
            SetConfigValue(penaltyLastCol, leagueId, (float)0);
            SetConfigValue(initialEloCol, leagueId, (float)1500);
            SetConfigValue(minEloCol, leagueId, (float)1200);
            SetConfigValue(baseEloChangeDampeningCol, leagueId, (float)10);
            SetConfigValue(eloChangeStartRatioCol, leagueId, (float)1);
            SetConfigValue(eloChangeEndRatioCol, leagueId, (float)0.2);
            SetConfigValue(trialPeriodDurationCol, leagueId, 80);

            return leagueId;
        }

        private static int CreateLeague(bool countPoints, bool useEloSystem)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {tableName} ({countPointsCol}, {useEloSystemCol}) "
                + $"OUTPUT INSERTED.{idCol} "
                + $"VALUES (@countPoints, @useElo) ";

                command.Parameters.Add(new SqlParameter("@countPoints", SqlDbType.Bit)
                {
                    Value = countPoints
                });
                command.Parameters.Add(new SqlParameter("@useElo", SqlDbType.Bit)
                {
                    Value = useEloSystem
                });
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
