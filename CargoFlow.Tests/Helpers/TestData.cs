using CargoFlow.Domain;

namespace CargoFlow.Tests.Helpers;

public static class TestData
{
    public static Customer CreateCustomer(string name = "ACME GmbH")
    {
        return new Customer(name);
    }

    public static Shipment CreateShipment(
        Customer customer,
        string originCity = "Cologne",
        string destinationCity = "Munich",
        decimal weight = 850)
    {
        return new Shipment(
            customer,
            new Address("Germany", originCity),
            new Address("Germany", destinationCity),
            weight
        );
    }
}
