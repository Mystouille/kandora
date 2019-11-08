using DSharpPlus.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Kandora
{
    class GlobalDb
    {
        public static void Commit()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = "COMMIT;";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }
        public static void Rollback()
        {
            var dbCon = DBConnection.Instance();
            if (dbCon.IsConnect())
            {
                using var command = SqlClientFactory.Instance.CreateCommand();
                command.Connection = dbCon.Connection;
                command.CommandText = "ROLLBACK;";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            throw (new DbConnectionException());
        }

    }
}
