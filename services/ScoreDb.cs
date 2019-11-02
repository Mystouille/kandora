using DSharpPlus.Entities;
using System;
using System.Data.SqlClient;
using DT = System.Data;

namespace Kandora
{
    class ScoreDb
    {
        private static readonly string GameTableName = "[dbo].[Game]";
        private static readonly string User1Id = "user1Id";
        private static readonly string User1Signed = "user1Signed";
        private static readonly string User2Id = "user2Id";
        private static readonly string User2Signed = "user2Signed";
        private static readonly string User3Id = "user3Id";
        private static readonly string User3Signed = "user3Signed";
        private static readonly string User4Id = "user4Id";
        private static readonly string User4Signed = "user4Signed";
        private static readonly string Timestamp = "timestamp";
        private static readonly string IdCol = "Id";

        public static bool RecordGame(DiscordMember[] members, bool signed = false)
        {
            var dbCon = DBConnection.Instance();
            bool touchedRecord = false;
            if (members[0] == members[1] || members[0] == members[2] || members[0] == members[3]
                || members[1] == members[2] || members[1] == members[3]
                || members[2] == members[3])
            {
                throw (new Exception("Cancelled. Need 4 different members"));
            }
            string bitSigned = signed ? "1" : "0";
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"INSERT INTO {GameTableName} ({User1Id}, {User2Id}, {User3Id}, {User4Id}, {User1Signed}, {User2Signed}, {User3Signed}, {User4Signed}) " +
                    $"VALUES ({members[0].Id}, {members[1].Id}, {members[2].Id}, {members[3].Id}, {bitSigned}, {bitSigned}, {bitSigned}, {bitSigned});";
                command.CommandType = DT.CommandType.Text;

                touchedRecord = command.ExecuteNonQuery() > 0;
            }
            return touchedRecord;
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
                command.CommandType = DT.CommandType.Text;
                try
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ulong user1Id = Convert.ToUInt64(reader.GetDecimal(1));
                        ulong user2Id = Convert.ToUInt64(reader.GetDecimal(2));
                        ulong user3Id = Convert.ToUInt64(reader.GetDecimal(3));
                        ulong user4Id = Convert.ToUInt64(reader.GetDecimal(4));
                        bool user1Signed = reader.GetBoolean(5);
                        bool user2Signed = reader.GetBoolean(6);
                        bool user3Signed = reader.GetBoolean(7);
                        bool user4Signed = reader.GetBoolean(8);

                        reader.Close();
                        return new Game(id, user1Id, user2Id, user3Id, user4Id, user1Signed, user2Signed, user3Signed, user4Signed);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine($"getLastRecordedGame: Couldn't get the last game");
            return null;
        }

        public static ulong[] GetUserIdsFromGame(int id)
        {
            var dbCon = DBConnection.Instance();
            ulong[] ids = new ulong[4];
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {User1Id}, {User2Id}, {User3Id}, {User4Id} " +
                    $"FROM {GameTableName}" +
                    $"WHERE {IdCol} = {id}";
                command.CommandType = DT.CommandType.Text;
                try
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ids[0] = Convert.ToUInt64(reader.GetDecimal(0));
                        ids[1] = Convert.ToUInt64(reader.GetDecimal(1));
                        ids[2] = Convert.ToUInt64(reader.GetDecimal(2));
                        ids[3] = Convert.ToUInt64(reader.GetDecimal(3));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            if(ids.Length != 4)
            {
                throw (new Exception($"getUsersFromGame: Fetched {ids.Length} ids from DB when we needed 4"));
            }
            return ids;
        }


        internal static bool SetMahjsoulId(int id, string value)
        {
            return UpdateColumnInTable<string>("mahjsoulId", value, id);
        }

        internal static bool SignGameByUser(int id, ulong userId)
        {
            var ids = GetUserIdsFromGame(id);
            int i = 0;
            while (ids[i] != userId && i<4)
            {
                i++;
            }
            if (i >= 4)
            {
                throw (new Exception("Cancelled. It seems like you didn't play that game."));
            }
            return SignGameByUserPos(id, i);
        }

        internal static bool SignGameByUserPos(int id, int userPos)
        {
            return UpdateColumnInTable<string>($"user{userPos}Signed", "1", id);
        }

        private static bool UpdateColumnInTable<T>(string columnName, T value, int id)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = string.Format("UPDATE {0} SET {1} = {2} WHERE Id = ${3}", GameTableName, columnName, value, id);
                    command.CommandType = DT.CommandType.Text;

                    return command.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }
    }
}
