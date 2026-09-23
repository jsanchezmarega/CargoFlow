using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using CargoFlow.Domain;

namespace CargoFlow.Data;

public class CargoFlowDbContext : DbContext
{
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Customer> Customers{ get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Env.TraversePath().Load();

        var host = Environment.GetEnvironmentVariable("DB_HOST")
            ?? throw new InvalidOperationException("DB_HOST is not set.");

        var port = Environment.GetEnvironmentVariable("DB_PORT")
            ?? throw new InvalidOperationException("DB_PORT is not set.");

        var database = Environment.GetEnvironmentVariable("DB_NAME")
            ?? throw new InvalidOperationException("DB_NAME is not set.");

        var user = Environment.GetEnvironmentVariable("DB_USER")
            ?? throw new InvalidOperationException("DB_USER is not set.");

        var password = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD is not set.");

        optionsBuilder.UseSqlServer(
            $"Server={host},{port};" +
            $"Database={database};" +
            $"User Id={user};" +
            $"Password={password};" +
            $"TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>()
            .ComplexProperty(s => s.Origin);

        modelBuilder.Entity<Shipment>()
            .ComplexProperty(s => s.Destination);
    }
}