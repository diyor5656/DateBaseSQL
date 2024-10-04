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
                                Class1.UpdateColumnValue(connectionString);
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
                Select.GetTableData(connectionString);



                Console.Write("IDni kiriting: ");
                string idValue = Console.ReadLine();

                
                Console.Write("Ustun nomini kiriting: ");
                string columnName = Console.ReadLine();

                Console.Write("Ustun qiymatini kiriting: ");
                string columnValue = Console.ReadLine();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query;
                    if (string.IsNullOrEmpty(idValue))
                    {
                        
                        query = $"INSERT INTO \"{tableName}\" (\"{columnName}\") VALUES (@columnValue)";
                    }
                    else
                    {
                        
                        query = $"INSERT INTO \"{tableName}\" (\"Id\", \"{columnName}\") VALUES (@idValue, @columnValue)";
                    }

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        
                        if (!string.IsNullOrEmpty(idValue))
                        {
                            command.Parameters.AddWithValue("@idValue", int.Parse(idValue));
                        }
                        command.Parameters.AddWithValue("@columnValue", columnValue);

                      
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{rowsAffected} qator {tableName} jadvaliga qo'shildi.");
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
                Console.WriteLine($"Xatolik: {ex.Message}");
                Console.ResetColor();
            }
        }



        public static void DeleteFromColumn(string connectionString)
        {
            Console.Clear();
            try
            {
               
                Select.GetTableNames(connectionString);
                Console.Write("Table nomini kiriting: ");
                string tableName = Console.ReadLine();

                if (Select.NotExist(connectionString, tableName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bunday nomdagi Table yuq!!!");
                    Console.ResetColor();
                }
                else
                {
                    Select.GetTableData(connectionString);

                    Console.Write("O'chirmoqchi bo'lgan qatorning id sini kiriting: ");
                    string rowId = Console.ReadLine();

                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = $"DELETE FROM \"{tableName}\" WHERE id = @rowId";
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@rowId", rowId);

                            int rowsAffected = command.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{rowsAffected} row(lar) o'chirildi {tableName}.");
                        }

                        connection.Close();
                    }
                }
            }
            catch (NpgsqlException npgEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Npgsqlda xato: {npgEx.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Xatolik: {ex.Message}");
                Console.ResetColor();
            }
        }




        public static void UpdateColumnValue(string connectionString)
        {
            Console.Clear();
            try
            {
                Select.GetTableNames(connectionString);
                Console.Write("Table nomini kiriting: ");
                string tableName = Console.ReadLine();

                if (Select.NotExist(connectionString, tableName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bunday nomdagi Table yuq!!!");
                    Console.ResetColor();
                }
                else
                {
                    Select.GetTableData(connectionString);

                    Console.Write("Yangilamoqchi bo'lgan qatorning id ni kiriting: ");
                    string rowId = Console.ReadLine();

                    Console.Write("Yangilamoqchi bo'lgan ustun nomini kiriting: ");
                    string columnName = Console.ReadLine();

                    Console.Write("Yangi qiymatni kiriting: ");
                    string newValue = Console.ReadLine();

                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = $"UPDATE \"{tableName}\" SET \"{columnName}\" = @newValue WHERE id = @rowId";
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@newValue", newValue);
                            command.Parameters.AddWithValue("@rowId", rowId);

                            int rowsAffected = command.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{rowsAffected} row(lar) yangilandi {tableName}.");
                        }

                        connection.Close();
                    }
                }
            }
            catch (NpgsqlException npgEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Npgsqlda xato: {npgEx.Message}");
                Console.ResetColor();
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

