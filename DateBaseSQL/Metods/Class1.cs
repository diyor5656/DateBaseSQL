using DateBaseSQL.Metods;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateBaseSQL.Metods
{
    public partial class Class1
    {
        public static void Elements(string connectionString)
        {
            Select.GetTableNames(connectionString);

            {
                bool exit = false;
                int selectedIndex = 0;
                List<string> Elementlar = new List<string>
        {
            "        Columnga element qoshish         ",
            "      Column elementini yangilash         ",
            "          Columndan o'chirish          ",
            "          Qatorlarni O'qish          ",
            "               Orqaga                     "
        };

                while (!exit)
                {
                    Console.Clear();
                    for (int i = 0; i < Elementlar.Count; i++)
                    {
                        if (i == selectedIndex)
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        Console.WriteLine(Elementlar[i]);
                        Console.ResetColor();
                    }

                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        selectedIndex = (selectedIndex + 1) % Elementlar.Count;
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        selectedIndex = (selectedIndex - 1 + Elementlar.Count) % Elementlar.Count;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        switch (selectedIndex)
                        {
                            case 0:
                                Class1.InsertIntoColumn(connectionString);
                                break;
                            case 1:
                                Class1.UpdateTable(connectionString);
                                break;
                            case 2:
                                Class1.DeleteFromColumn(connectionString);
                                break;
                            case 3:
                                Select.GetTableData(connectionString);
                                break;
                            case 4:
                                exit = true;
                                break;
                            default:
                                Console.WriteLine("Xato tanlov!!!");
                                break;
                        }

                        Console.ReadKey();
                    }
                }
                Console.Clear();
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

        public static void InsertIntoColumn(string connectionString)
        {
            Console.Clear();
            try
            {
                
                Select.GetTableNames(connectionString);
                Console.Write("Jadval nomini kiriting: ");
                string tableName = Console.ReadLine();
                Select.GetTableData1(connectionString, tableName);

                Dictionary<string, string> columns = GetTableColumns(connectionString, tableName);

                Dictionary<string, object> columnsToInsert = new Dictionary<string, object>();

                
                foreach (var column in columns)
                {
                    Console.Write($"{column.Key} (tur: {column.Value}) uchun qiymat kiriting: ");
                    string columnValue = Console.ReadLine();

                    
                    switch (column.Value.ToLower())
                    {
                        case "smallint":
                            if (short.TryParse(columnValue, out short smallIntValue))
                            {
                                columnsToInsert.Add(column.Key, smallIntValue);  
                            }
                            else
                            {
                                Console.WriteLine($"Xato: {column.Key} uchun noto'g'ri qiymat kiritildi.");
                                return; 
                            }
                            break;

                        case "integer":
                            if (int.TryParse(columnValue, out int intValue))
                            {
                                columnsToInsert.Add(column.Key, intValue);
                            }
                            else
                            {
                                Console.WriteLine($"Xato: {column.Key} uchun noto'g'ri qiymat kiritildi.");
                                return;
                            }
                            break;

                        case "character varying":
                        case "text":
                            columnsToInsert.Add(column.Key, columnValue);  
                            break;

                        default:
                            columnsToInsert.Add(column.Key, columnValue);  
                            break;
                    }
                }

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    
                    StringBuilder query = new StringBuilder($"INSERT INTO \"{tableName}\" (");
                    foreach (var column in columnsToInsert.Keys)
                    {
                        query.Append($"\"{column}\", ");
                    }
                    query.Length -= 2;
                    query.Append(") VALUES (");

                    foreach (var column in columnsToInsert.Keys)
                    {
                        query.Append($"@{column}, ");
                    }
                    query.Length -= 2; 
                    query.Append(");");

                    using (var command = new NpgsqlCommand(query.ToString(), connection))
                    {
                        foreach (var column in columnsToInsert)
                        {
                            command.Parameters.AddWithValue($"@{column.Key}", column.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{rowsAffected} qator qo'shildi {tableName} jadvaliga.");
                    }

                    connection.Close();
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Xato: Kiritilgan qiymat ustunning turi bilan mos emas.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik: {ex.Message}");
                Console.ResetColor();
            }
        }

        
        public static Dictionary<string, string> GetTableColumns(string connectionString, string tableName)
        {
            var columns = new Dictionary<string, string>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = $@"
            SELECT column_name, data_type 
            FROM information_schema.columns 
            WHERE table_name = @tableName
            ORDER BY ordinal_position";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(0);
                            string columnType = reader.GetString(1);
                            columns.Add(columnName, columnType);
                        }
                    }
                }

                connection.Close();
            }

            return columns;
        }

        
        public static string GetColumnType(NpgsqlConnection connection, string tableName, string columnName)
        {
            string columnType = string.Empty;

            string query = $"SELECT data_type FROM information_schema.columns WHERE table_name = @tableName AND column_name = @columnName";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                command.Parameters.AddWithValue("@columnName", columnName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        columnType = reader.GetString(0).ToLower();
                    }
                }
            }

            return columnType;
        }






        public static void DeleteFromTable(string connectionString)
        {
            Console.Clear();
            try
            {
                Select.GetTableNames(connectionString);
                Console.Write("Jadval nomini kiriting: ");
                string tableName = Console.ReadLine();
                Select.GetTableData1(connectionString, tableName);

                Console.Write("Qaysi qatorni o'chirmoqchisiz (masalan, id = 1): ");
                string condition = Console.ReadLine();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"DELETE FROM \"{tableName}\" WHERE {condition};";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{rowsAffected} qator o'chirildi {tableName} jadvalida.");
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik: {ex.Message}");
                Console.ResetColor();
            }
        }






        public static void UpdateTable(string connectionString)
        {
            Console.Clear();
            try
            {
                Select.GetTableNames(connectionString);
                Console.Write("Jadval nomini kiriting: ");
                string tableName = Console.ReadLine();
                Select.GetTableData1(connectionString, tableName);

                Dictionary<string, string> columns = GetTableColumns(connectionString, tableName);

                Console.Write("Qaysi qatorni yangilamoqchisiz (masalan, id = 1): ");
                string condition = Console.ReadLine();

                Dictionary<string, object> columnsToUpdate = new Dictionary<string, object>();

                foreach (var column in columns)
                {
                    Console.Write($"{column.Key} (tur: {column.Value}) uchun yangi qiymat kiriting: ");
                    string columnValue = Console.ReadLine();

                    switch (column.Value.ToLower())
                    {
                        case "smallint":
                            if (short.TryParse(columnValue, out short smallIntValue))
                            {
                                columnsToUpdate.Add(column.Key, smallIntValue);
                            }
                            else
                            {
                                Console.WriteLine($"Xato: {column.Key} uchun noto'g'ri qiymat kiritildi.");
                                return;
                            }
                            break;

                        case "integer":
                            if (int.TryParse(columnValue, out int intValue))
                            {
                                columnsToUpdate.Add(column.Key, intValue);
                            }
                            else
                            {
                                Console.WriteLine($"Xato: {column.Key} uchun noto'g'ri qiymat kiritildi.");
                                return;
                            }
                            break;

                        case "character varying":
                        case "text":
                            columnsToUpdate.Add(column.Key, columnValue);
                            break;

                        default:
                            columnsToUpdate.Add(column.Key, columnValue);
                            break;
                    }
                }

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    StringBuilder query = new StringBuilder($"UPDATE \"{tableName}\" SET ");
                    foreach (var column in columnsToUpdate)
                    {
                        query.Append($"\"{column.Key}\" = @{column.Key}, ");
                    }
                    query.Length -= 2;
                    query.Append($" WHERE {condition};");

                    using (var command = new NpgsqlCommand(query.ToString(), connection))
                    {
                        foreach (var column in columnsToUpdate)
                        {
                            command.Parameters.AddWithValue($"@{column.Key}", column.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{rowsAffected} qator yangilandi {tableName} jadvalida.");
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik: {ex.Message}");
                Console.ResetColor();
            }
        }



    }
}

