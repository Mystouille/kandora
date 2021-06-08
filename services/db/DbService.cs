using kandora.bot.exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.services.db
{
    class DbService
    {
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
        protected static void UpdateFieldInTable<T>(string tableName, string columnName, string forId, T newValue)
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using (var command = SqlClientFactory.Instance.CreateCommand())
                {
                    command.Connection = dbCon.Connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"UPDATE {tableName} SET {columnName} = @newValue WHERE Id = {forId}";

                    SqlDbType type = SqlDbType.VarChar;
                    if (newValue.GetType() == typeof(int))
                    {
                        type = SqlDbType.Int;
                    }
                    else if (newValue.GetType() == typeof(bool))
                    {
                        type = SqlDbType.Bit;
                    }
                    else if (newValue.GetType() == typeof(float))
                    {
                        type = SqlDbType.Float;
                    }
                    command.Parameters.Add(new SqlParameter("@newValue", type)
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
    }
}
