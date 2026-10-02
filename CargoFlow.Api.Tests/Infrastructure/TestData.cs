using CargoFlow.Data;
using CargoFlow.Domain;

namespace CargoFlow.Api.Tests.Infrastructure;

public static class TestData
{
    public static async Task<Customer> CreateCustomerAsync(
        CargoFlowDbContext dbContext,
        string name = "Test Customer",
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer(name);

        dbContext.Customers.Add(customer);

        await dbContext.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public static async Task<Shipment> CreateShipmentAsync(
        CargoFlowDbContext dbContext,
        string customerName = "Test Customer",
        string originCity = "Cologne",
        string destinationCity = "Berlin",
        decimal weight = 125.5m,
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer(customerName);

        var shipment = new Shipment(
            customer,
            new Address("Germany", originCity),
            new Address("Germany", destinationCity),
            weight);

        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(cancellationToken);

        return shipment;
    }
}