using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Kandora
{
    public class KandoraCommands
    {/*
        [Command("hand")]
        public async Task Hand(CommandContext ctx, params string[] handBits)
        {
            var parser = new HandParser(ctx.Client);
            var hand = string.Join("", handBits);
            var result = parser.getHandEmojiCode(hand);
            await ctx.RespondAsync(result);
        }

        [Command("register")]
        public async Task Register(CommandContext ctx, string mahjsoulId)
        {

            await ctx.RespondAsync(ctx.Member.Email + " " + ctx.Member.DisplayName);
        }

        [Command("test")]
        public async Task Test(CommandContext ctx, string mahjsoulId)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["ClientToken"].ConnectionString;
            string queryString = "SELECT Id, displayName, mahjsoulId from dbo.User";
            using SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(queryString, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}",
                        reader[0], reader[1], reader[2]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();

        }*/
    }
}