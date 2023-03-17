using kandora.bot.exceptions;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace kandora.bot.services.db
{
    class DbService
    {
        protected static DbDataReader Reader = null;
        public static void Begin(string transaction)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"START TRANSACTION READ WRITE;";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }
        public static void Commit(string transaction)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"COMMIT TRANSACTION;";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }
        public static void Rollback(string transaction)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandText = $"ROLLBACK TRANSACTION;";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }
        protected static void UpdateFieldInTable<T1, T2>(string tableName, string columnName, T1 forId, T2 newValue)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.CommandType = CommandType.Text;
                command.CommandText = $"UPDATE {tableName} SET {columnName} = @newValue WHERE Id = @forId";

                NpgsqlDbType newValueType = GetSqlType(newValue);
                NpgsqlDbType forIdType = GetSqlType(forId);
                command.Parameters.AddWithValue("@forId", forIdType, forId);
                command.Parameters.AddWithValue("@newValue", newValueType, newValue);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
                return;
            }
            throw (new DbConnectionException());
        }

        private static NpgsqlDbType GetSqlType<T>(T val)
        {
            NpgsqlDbType type = NpgsqlDbType.Varchar;
            if (val.GetType() == typeof(int))
            {
                type = NpgsqlDbType.Integer;
            }
            else if (val.GetType() == typeof(bool))
            {
                type = NpgsqlDbType.Boolean;
            }
            else if (val.GetType() == typeof(double))
            {
                type = NpgsqlDbType.Double;
            }
            else if (val.GetType() == typeof(DateTime))
            {
                type = NpgsqlDbType.Timestamp;
            }
            return type;
        }

        protected static int GetMaxValueFromCol(string tableName, string colName)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = new NpgsqlCommand("", dbCon.Connection);
                command.Connection = dbCon.Connection;
                command.CommandText = $"SELECT {colName} FROM {tableName}";
                command.CommandType = CommandType.Text;
                Reader = command.ExecuteReader();
                int nb = 0;
                while (Reader.Read())
                {
                    nb++;
                }
                Reader.Close();
                return nb;
            }
            throw (new DbConnectionException());
        }
    }
}
