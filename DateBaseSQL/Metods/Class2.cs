using DateBaseSQL.Metods;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateBaseSQL.Metods
{
    public partial class Class2
    {
        public static void DropTable(string connectionString)
        {
            Select.GetTableNames(connectionString);
            try
            {
               
                Console.Write("O'chirmoqchi bo'lgan jadval nomini kiriting: ");
                string tableName = Console.ReadLine();

                if (string.IsNullOrEmpty(tableName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Jadval nomi kiritilmadi.");
                    Console.ResetColor();
                    return;
                }

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    
                    string query = $"DROP TABLE IF EXISTS \"{tableName}\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        
                        command.ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"'{tableName}' jadvali muvaffaqiyatli o'chirildi.");
                        Console.ResetColor();
                    }

                    connection.Close();
                }
            }
            catch (NpgsqlException npgEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Npgsql xatosi: {npgEx.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik yuz berdi: {ex.Message}");
                Console.ResetColor();
            }
        }

    }
}

   
        

    

