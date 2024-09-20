using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateBaseSQL.Metods
{
    public partial class Select
    {

        public static void SelectProducts(string connectionString, string selectQuery)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connection opened");

                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string productName = reader.GetString(1);
                        Console.WriteLine($"Product: {productName}");
                    }
                }

                connection.Close();
                Console.WriteLine("Connection closed");
            }
        }
    }
}
