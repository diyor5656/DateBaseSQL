using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateBaseSQL.Metods;

namespace DateBaseSQL.Metods
{
    public partial class Class1
    {
        public static void Elements(string connectionString)
        {
            Select.GetTableNames(connectionString);
            Console.WriteLine("Tableni tanlang: ");
            string tableName = Console.ReadLine();
            if (Select.NotExist(connectionString, tableName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bunday nomdagi Table yuq!!!");
                Console.ResetColor();
            }
            else
            {
                Select.GetTableData(connectionString, tableName);

            }
            

        }

        public static void GetColumnData(string connectionString, string tableName, string columnName)
        {

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT \"*\" FROM \"{tableName}\"";
                        using (var reader = command.ExecuteReader())
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"\n'{tableName}' jadvalidagi '{columnName}' ustunidagi ma'lumotlar:");
                            while (reader.Read())
                            {
                                Console.WriteLine(reader[columnName].ToString());
                            }
                            Console.ResetColor();
                        }
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

        public static void UpdateData(string connectionString, string tableName, string conditionColumn, object conditionValue, Dictionary<string, object> newData)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL UPDATE so'rovini yaratish
                    var setClauses = string.Join(", ", newData.Keys.Select(key => $"\"{key}\" = @{key}"));
                    string query = $"UPDATE \"{tableName}\" SET {setClauses} WHERE \"{conditionColumn}\" = @conditionValue";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Parametrlarni qo'shish
                        foreach (var entry in newData)
                        {
                            command.Parameters.AddWithValue($"@{entry.Key}", entry.Value);
                        }
                        command.Parameters.AddWithValue("@conditionValue", conditionValue);

                        // So'rovni bajarish
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} qatorlar yangilandi.");
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

