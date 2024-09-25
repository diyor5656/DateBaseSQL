using Npgsql;
using System;
using System.Collections.Generic;

namespace DateBaseSQL.Metods
{
    public partial class Alter
    {

        public static void AlterTable(string connectionString)
        {
           
            try
            {
                Select.GetTableNames(connectionString);
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Select.GetTableNames(connectionString);
                    Console.Write("O'zgartirmoqchi bo'lgan Table nomini kiriting: ");
                    string tableName = Console.ReadLine();
                    Select.GetTableColumns(connectionString, tableName);
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("\nJadvalni o'zgartirish uchun quyidagi variantlardan birini tanlang:");
                    Console.WriteLine("1. Column qo'shish");
                    Console.WriteLine("2. Columnni o'chirish");
                    Console.WriteLine("3. Columnni qayta nomlash");
                    Console.WriteLine("4. Tableni qayta nomlash");
                    Console.WriteLine("5. Columnni turini o'zgartirish");

                    string option = Console.ReadLine();

                    switch (option)
                    {

                        case "1":
                            Console.Clear();
                            Console.Write("Qo'shmoqchi bo'lgan column nomini kiriting: ");
                            string newColumnName = Console.ReadLine();

                            Console.WriteLine("Column type'ini tanlang:");
                            Console.WriteLine("1. VARCHAR(255)");
                            Console.WriteLine("2. INTEGER");
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
                                default:
                                    Console.WriteLine("Noto'g'ri tanlov. Standart VARCHAR(255) tanlandi.");
                                    dataType = "VARCHAR(255)";
                                    break;
                            }

                            string addColumnQuery = $"ALTER TABLE {tableName} ADD COLUMN {newColumnName} {dataType};";

                            ExecuteNonQuery(connection, addColumnQuery);
                            Console.WriteLine($"Column '{newColumnName}' muvaffaqiyatli qo'shildi.");
                            break;

                        case "2":
                            Console.Clear();
                            Console.Write("O'chirmoqchi bo'lgan column nomini kiriting: ");
                            string dropColumnName = Console.ReadLine();

                            string dropColumnQuery = $"ALTER TABLE {tableName} DROP COLUMN {dropColumnName};";

                            ExecuteNonQuery(connection, dropColumnQuery);
                            Console.WriteLine($"Column '{dropColumnName}' muvaffaqiyatli o'chirildi.");
                            break;

                        case "3":
                            Console.Clear();
                            Console.Write("Joriy column nomini kiriting: ");
                            string oldColumnName = Console.ReadLine();

                            Console.Write("Yangi column nomini kiriting: ");
                            string newColumnNameForRename = Console.ReadLine();

                            string renameColumnQuery = $"ALTER TABLE {tableName} RENAME COLUMN {oldColumnName} TO {newColumnNameForRename};";

                            ExecuteNonQuery(connection, renameColumnQuery);
                            Console.WriteLine($"Column '{oldColumnName}' muvaffaqiyatli '{newColumnNameForRename}' deb qayta nomlandi.");
                            break;

                        case "4":
                            Console.Clear();
                            Console.Write("Yangi Table nomini kiriting: ");
                            string newTableName = Console.ReadLine();

                            string renameTableQuery = $"ALTER TABLE {tableName} RENAME TO {newTableName};";

                            ExecuteNonQuery(connection, renameTableQuery);
                            Console.WriteLine($"Table '{tableName}' muvaffaqiyatli '{newTableName}' deb qayta nomlandi.");
                            break;

                        case "5":
                            Console.Clear();
                            Console.Write("Turini o'zgartirmoqchi bo'lgan column nomini kiriting: ");
                            string columnToChange = Console.ReadLine();

                            Console.WriteLine("Column type'ini tanlang:");
                            Console.WriteLine("1. VARCHAR(255)");
                            Console.WriteLine("2. INTEGER");
                            Console.Write("Tanlash uchun raqamni kiriting: ");
                            string newDataType = "";

                            switch (Console.ReadLine())
                            {
                                case "1":
                                    dataType = "VARCHAR(255)";
                                    break;
                                case "2":
                                    dataType = "INTEGER";
                                    break;
                                default:
                                    Console.WriteLine("Noto'g'ri tanlov. Standart VARCHAR(255) tanlandi.");
                                    dataType = "VARCHAR(255)";
                                    break;
                            }
                            string changeColumnTypeQuery = $"ALTER TABLE {tableName} ALTER COLUMN {columnToChange} TYPE {newDataType};";

                            ExecuteNonQuery(connection, changeColumnTypeQuery);
                            Console.WriteLine($"Column '{columnToChange}' turi '{newDataType}' ga muvaffaqiyatli o'zgartirildi.");
                            break;

                        default:
                            Console.WriteLine("Noto'g'ri variant tanlandi.");
                            break;
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


        private static void ExecuteNonQuery(NpgsqlConnection connection, string query)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }


        public static void GetAbout(string connectionString)
        {
            Console.Clear();

            Console.Write("Jadval nomini kiriting (ko'rsatish uchun): ");
            string tableName = Console.ReadLine();

            string query = $"SELECT * FROM {tableName};";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    try
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetName(i) + "\t");
                            }
                            Console.WriteLine();

                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write(reader[i].ToString() + "\t");
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Xatolik: " + ex.Message);
                        Console.ResetColor();
                    }
                }

                connection.Close();
            }
        }

    }
}
