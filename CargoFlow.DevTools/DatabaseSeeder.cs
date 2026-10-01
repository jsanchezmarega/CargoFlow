using CargoFlow.Data;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.DevTools;

public class DatabaseSeeder
{
    private readonly CargoFlowDbContext _dbContext;

    public DatabaseSeeder(CargoFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Customers.AnyAsync())
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return;
        }

        var acme = new Customer("ACME GmbH");
        var globex = new Customer("Globex AG");
        var initech = new Customer("Initech GmbH");

        var shipments = new[]
        {
            new Shipment(
                acme,
                new Address("Germany", "Cologne"),
                new Address("Germany", "Munich"),
                850
            ),
            new Shipment(
                acme,
                new Address("Germany", "Hamburg"),
                new Address("Germany", "Berlin"),
                420
            ),
            new Shipment(
                globex,
                new Address("Germany", "Frankfurt"),
                new Address("Germany", "Düsseldorf"),
                1250
            ),
            new Shipment(
                globex,
                new Address("Germany", "Stuttgart"),
                new Address("Germany", "Leipzig"),
                675
            ),
            new Shipment(
                initech,
                new Address("Germany", "Bonn"),
                new Address("Germany", "Dresden"),
                300
            )
        };

        shipments[1].StartTransit();

        shipments[2].StartTransit();
        shipments[2].MarkAsDelivered();

        shipments[3].Cancel();

        _dbContext.Customers.AddRange(acme, globex, initech);
        _dbContext.Shipments.AddRange(shipments);

        await _dbContext.SaveChangesAsync();

        Console.WriteLine(
            $"Seeded {3} customers and {shipments.Length} shipments."
        );
    }
}