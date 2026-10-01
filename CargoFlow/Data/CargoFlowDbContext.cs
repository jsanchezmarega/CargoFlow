using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Data;

public class CargoFlowDbContext : DbContext
{
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public CargoFlowDbContext(
        DbContextOptions<CargoFlowDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>()
            .ComplexProperty(s => s.Origin);

        modelBuilder.Entity<Shipment>()
            .ComplexProperty(s => s.Destination);
    }
}