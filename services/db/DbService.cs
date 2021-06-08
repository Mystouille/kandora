using kandora.bot.exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"BEGIN TRANSACTION {transaction};";
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"COMMIT TRANSACTION {transaction};";
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
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = $"ROLLBACK TRANSACTION {transaction};";
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
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {tableName} SET {columnName} = @newValue WHERE Id = @forId";

                    SqlDbType newValueType = GetSqlType(newValue);
                    SqlDbType forIdType = GetSqlType(forId);
                    command.Parameters.Add(new SqlParameter("@forId", forIdType)
                    {
                        Value = forId
                    });
                    command.Parameters.Add(new SqlParameter("@newValue", newValueType)
                    {
                        Value = newValue
                    });
                    command.CommandType = CommandType.Text;

                    command.ExecuteNonQuery();
                    return;
                }
            }
            throw (new DbConnectionException());
        }

        private static SqlDbType GetSqlType<T>(T val)
        {
            SqlDbType type = SqlDbType.VarChar;
            if (val.GetType() == typeof(int))
            {
                type = SqlDbType.Int;
            }
            else if (val.GetType() == typeof(bool))
            {
                type = SqlDbType.Bit;
            }
            else if (val.GetType() == typeof(float))
            {
                type = SqlDbType.Float;
            }
            return type;
        }

        protected static int GetMaxValueFromCol(string tableName, string colName)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
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
