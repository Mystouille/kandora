using DSharpPlus.Entities;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Kandora
{
    class ScoreDb
    {
        public static DbDataReader Reader = null;
        private const string GameTableName = "[dbo].[Game]";
        private const string RankingTableName = "[dbo].[Ranking]";
        private const string RankingTableGameIdCol = "gameId";
        private const string User1Id = "user1Id";
        private const string User1Signed = "user1Signed";
        private const string User2Id = "user2Id";
        private const string User2Signed = "user2Signed";
        private const string User3Id = "user3Id";
        private const string User3Signed = "user3Signed";
        private const string User4Id = "user4Id";
        private const string User4Signed = "user4Signed";
        private const string Timestamp = "timestamp";
        private const string IdCol = "Id";

        public static Game RecordGame(ulong[] members, ulong sourceMember, bool signed = false)
        {
            var dbCon = DBConnection.Instance();

            if (members[0] == members[1] || members[0] == members[2] || members[0] == members[3]
                || members[1] == members[2] || members[1] == members[3]
                || members[2] == members[3])
            {
                throw (new NotEnoughUsersException());
            }

            string bitSigned1 = (signed || sourceMember == members[0]) ? "1" : "0";
            string bitSigned2 = (signed || sourceMember == members[1]) ? "1" : "0";
            string bitSigned3 = (signed || sourceMember == members[2]) ? "1" : "0";
            string bitSigned4 = (signed || sourceMember == members[3]) ? "1" : "0";

            int gameId = GetMaxId() + 1;

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SET IDENTITY_INSERT {GameTableName} ON \n" +
                    $"INSERT INTO {GameTableName} ({IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}) " +
                    $"VALUES ({gameId}, {members[0]}, {members[1]}, {members[2]}, {members[3]}, {bitSigned1}, {bitSigned2}, {bitSigned3}, {bitSigned4});";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();

                return GetGame(gameId);
            }
            throw (new DbConnectionException());
        }

        public static void RevertGame(int gameId)
        {
            var dbCon = DBConnection.Instance();

            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"DELETE FROM {RankingTableName} WHERE {RankingTableGameIdCol} = {gameId}" +
                    $"DELETE FROM {GameTableName} WHERE {IdCol} = {gameId}";
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }

        private static int GetMaxId()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT MAX({IdCol}) FROM {GameTableName}";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.IsDBNull(0) ? 0 : Reader.GetInt32(0);
                    Reader.Close();
                    return id;
                }
            }
            throw (new DbConnectionException());
        }

        public static Game GetLastRecordedGame()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {Timestamp} = (SELECT MAX({Timestamp}) FROM {GameTableName})";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.GetInt32(0);
                    ulong user1Id = Convert.ToUInt64(Reader.GetDecimal(1));
                    ulong user2Id = Convert.ToUInt64(Reader.GetDecimal(2));
                    ulong user3Id = Convert.ToUInt64(Reader.GetDecimal(3));
                    ulong user4Id = Convert.ToUInt64(Reader.GetDecimal(4));
                    bool user1Signed = Reader.GetBoolean(5);
                    bool user2Signed = Reader.GetBoolean(6);
                    bool user3Signed = Reader.GetBoolean(7);
                    bool user4Signed = Reader.GetBoolean(8);

                    Reader.Close();
                    return new Game(id, user1Id, user2Id, user3Id, user4Id, user1Signed, user2Signed, user3Signed, user4Signed);
                }
            }
            throw (new DbConnectionException());
        }

        public static Game GetGame(int gameId)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {IdCol}, {User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}" +
                    $" FROM {GameTableName}" +
                    $" WHERE {IdCol} = @gameId";

                //sql injection protection
                command.Parameters.Add(new SqlParameter("@gameId", SqlDbType.Int)
                {
                    Value = gameId
                });

                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    int id = Reader.GetInt32(0);
                    ulong user1Id = Convert.ToUInt64(Reader.GetDecimal(1));
                    ulong user2Id = Convert.ToUInt64(Reader.GetDecimal(2));
                    ulong user3Id = Convert.ToUInt64(Reader.GetDecimal(3));
                    ulong user4Id = Convert.ToUInt64(Reader.GetDecimal(4));
                    bool user1Signed = Reader.GetBoolean(5);
                    bool user2Signed = Reader.GetBoolean(6);
                    bool user3Signed = Reader.GetBoolean(7);
                    bool user4Signed = Reader.GetBoolean(8);

                    Reader.Close();

                    if(id != gameId)
                    {
                        throw (new GetGameException($"Tried to fetch game n°{gameId} but got n°{id} instead."));
                    }
                    return new Game(id, user1Id, user2Id, user3Id, user4Id, user1Signed, user2Signed, user3Signed, user4Signed);
                }
                throw (new GetGameException($"Game n°{gameId} not found after creating it"));
            }
            throw (new DbConnectionException());
        }
               
        internal static void SetMahjsoulId(int id, string value)
        {
            UpdateColumnInTable("mahjsoulId", value, id);
        }

        internal static void SignGameByUser(int id, ulong userId)
        {
            var game = GetGame(id);
            game.TrySignGameByUser(userId);
            if (game.IsSigned)
            {
                RankingDb.UpdateRankings(game);
            }
        }

        internal static bool SignGameByUserPos(int id, int userPos)
        {
            return UpdateColumnInTable($"user{userPos}Signed", "1", id);
        }

        private static bool UpdateColumnInTable(string columnName, string value, int id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {GameTableName} SET {columnName} = {value} WHERE Id = {id}";
                    command.CommandType = CommandType.Text;

                    return command.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }
    }
}
