using CargoFlow.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CargoFlow.Api.Tests.Infrastructure;

public class CargoFlowWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<CargoFlowDbContext>(options =>
                options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider()
                .CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<CargoFlowDbContext>();

            dbContext.Database.EnsureCreated();
        });
    }

    public async Task ExecuteDbAsync(
        Func<CargoFlowDbContext, Task> action)
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CargoFlowDbContext>();

        await action(dbContext);
    }

    public async Task<T> ExecuteDbAsync<T>(
        Func<CargoFlowDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CargoFlowDbContext>();

        return await action(dbContext);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}