namespace CargoFlow.Domain;

public class Customer
{
    private Customer()
    {
    }

    public Customer(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name cannot be empty.",
                nameof(name)
            );
        }

        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; set; } = null!;
    public List<Shipment> Shipments { get; private set; } = new();
}
