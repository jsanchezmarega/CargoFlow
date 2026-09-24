namespace CargoFlow.Domain;

public class Customer
{
    private Customer()
    {
    }

    public Customer(string name)
    {
        this.Name = name;
    }
    public int Id { get; private set; }
    public string Name { get; set; } = null!;
    public List<Shipment> Shipments { get; private set; } = new();
}
