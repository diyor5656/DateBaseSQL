using Npgsql;
using System.Data;

namespace DateBaseSQL.Metods
{
    public partial class Select
    {

        public static void GetTableNames(string connectionString)
        {
            Console.Clear();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        var item = connection.GetSchema("Tables");
                        foreach (DataRow table in item.Rows)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(table["TABLE_NAME"]);
                        }

                    }
                    Console.Write("\nKo'rsatmoqchi bo'lgan Table nomini kiriting: ");
                    string tableName = Console.ReadLine();
                    GetTableData(connectionString, tableName);

                    Console.Write("\nQaysi Tabledagi columnlarni kormoqchisiz?: ");
                    string columnName = Console.ReadLine();
                    if (NotExist(connectionString, tableName))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bunday nomdagi Table yuq!!!");
                        Console.ResetColor();
                    }
                    else
                    {
                        GetTableColumns(connectionString, tableName);

                    }
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
        public static void GetTableColumns(string connectionString, string tableName)
        {
            Console.Clear();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        var schema = connection.GetSchema("Columns", new string[] { null, null, tableName, null });
                        Console.WriteLine($"\n'{tableName}' Tabledagi columnlar:");
                        foreach (DataRow column in schema.Rows)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(column["COLUMN_NAME"]);
                        }
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

        

        public static bool NotExist(string connectionString, string tableName)
        {
            bool tableNotExist = true;
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var schema = connection.GetSchema("Tables");

                    foreach (DataRow row in schema.Rows)
                    {
                        if (row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        {
                            tableNotExist = false;
                            break;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik yuz berdi: {ex.Message}");
                Console.ResetColor();
            }

            return tableNotExist;
        }


        public static void GetTableData(string connectionString, string tableName)
        {
            Console.Clear();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = $"SELECT * FROM \"{tableName}\"";

                        using (var reader = command.ExecuteReader())
                        {
                            // Ustun nomlarini chiqarish
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetName(i) + "\t");
                            }
                            Console.WriteLine();
                            Console.ResetColor();


                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write(reader.GetValue(i) + "\t");
                                }
                                Console.WriteLine();
                            }
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

    }
}
