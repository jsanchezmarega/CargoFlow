using CargoFlow.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Tests.Infrastructure;

public class TestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private TestDatabase(SqliteConnection connection)
    {
        _connection = connection;

        Options = new DbContextOptionsBuilder<CargoFlowDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    public DbContextOptions<CargoFlowDbContext> Options { get; }

    public static async Task<TestDatabase> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync(cancellationToken);

        var database = new TestDatabase(connection);

        await using var dbContext = database.CreateContext();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        return database;
    }

    public CargoFlowDbContext CreateContext()
    {
        return new CargoFlowDbContext(Options);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}