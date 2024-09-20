
using System;
using System.IO;
using Newtonsoft.Json;
using Npgsql;
using DateBaseSQL.Metods;

public static class Program
{
    static void Main(string[] args)
    {
       
        File.WriteAllText("appsettings.json", "{\n  \"ConnectionString\": {\n    \"PgConnection\": \"\"\n  }\n}");

        
        Console.Write("Host (masalan: localhost): ");
        string host = Console.ReadLine();

        Console.Write("Port (masalan: 5432): ");
        string port = Console.ReadLine();

        Console.Write("Database nomi (masalan: myDB): ");
        string database = Console.ReadLine();

        Console.Write("User ID (masalan: postgres): ");
        string userId = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

       
        string connectionString = $"Host={host};Port={port};Database={database};User Id={userId};Password={password};";

      
        var appSettings = new
        {
            ConnectionString = new
            {
                PgConnection = connectionString
            }
        };

        
        string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
        File.WriteAllText("appsettings.json", json);

       
        var loadedSettings = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("appsettings.json"));
        string pgConnection = loadedSettings.ConnectionString.PgConnection;

       
        var query = "SELECT * FROM Products";
        Select.SelectProducts(pgConnection, query);
    }

    
    
}
