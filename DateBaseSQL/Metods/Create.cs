using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateBaseSQL.Metods
{
    public partial class Create
    {
        public static void Createtable(string connectionString)
        {
            Console.Clear();
            Select.GetTableNames(connectionString);
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    Console.Write("Table nomini kiriting : ");
                    string tableName = Console.ReadLine();

                    List<string> columns = new List<string>();
                    bool addMoreColumns = true;

                    while (addMoreColumns)
                    {
                        Console.Write("Column nomini kiriting: ");
                        string columnName = Console.ReadLine();

                        Console.WriteLine("Column type'ini tanlang:");
                        Console.WriteLine("1. VARCHAR(255)");
                        Console.WriteLine("2. INTEGER");
                        Console.WriteLine("3. SERIAL");
                        Console.WriteLine("4. DECIMAL(10, 2)");
                        Console.Write("Tanlash uchun raqamni kiriting: ");
                        string dataType = "";

                        switch (Console.ReadLine())
                        {
                            case "1":
                                dataType = "VARCHAR(255)";
                                break;
                            case "2":
                                dataType = "INTEGER";
                                break;
                            case "3":
                                dataType = "SERIAL";
                                break;
                            case "4":
                                dataType = "DECIMAL(10, 2)";
                                break;
                            default:
                                Console.WriteLine("Noto'g'ri tanlov. Standart VARCHAR(255) tanlandi.");
                                dataType = "VARCHAR(255)";
                                break;
                        }

                        columns.Add($"{columnName} {dataType}");

                        Console.Write("Yana Column qo'shishni xohlaysizmi? (y/n): ");
                        string YesOrNo = Console.ReadLine();
                        addMoreColumns = YesOrNo.ToLower() == "y";
                    }

                    string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (\n" +
                                               string.Join(",\n", columns) +
                                               "\n);";

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = createTableQuery;
                        command.ExecuteNonQuery();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Table '{tableName}' muvaffaqiyatli yaratildi.");
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Xatolik yuz berdi: " + ex.Message);
                Console.ResetColor();
            }
        }

    }
}
