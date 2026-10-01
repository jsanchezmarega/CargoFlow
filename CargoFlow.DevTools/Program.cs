using CargoFlow.Data;
using CargoFlow.DevTools;
using Microsoft.EntityFrameworkCore;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

switch (args[0].ToLowerInvariant())
{
    case "seed":
        await SeedDatabaseAsync();
        return 0;

    default:
        Console.Error.WriteLine($"Unknown command: {args[0]}");
        PrintUsage();
        return 1;
}

static async Task SeedDatabaseAsync()
{
    var connectionString = DatabaseConfiguration.GetConnectionString();

    var options = new DbContextOptionsBuilder<CargoFlowDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    await using var dbContext = new CargoFlowDbContext(options);

    var seeder = new DatabaseSeeder(dbContext);

    await seeder.SeedAsync();
}

static void PrintUsage()
{
    Console.WriteLine("CargoFlow development tools");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project CargoFlow.DevTools -- <command>");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  seed    Seed the development database");
}