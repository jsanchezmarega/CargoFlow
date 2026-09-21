namespace CargoFlow.Domain;

class Shipment
{
    private Shipment()
    {
    }

    public Shipment(Customer customer, Address origin, Address destination, decimal weight)
    {
        this.Customer = customer;
        this.Origin = origin;
        this.Destination = destination;
        this.Weight = weight;
    }
    public int Id { get; private set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; private set; }
    public Address Origin { get; set; }
    public Address Destination { get; set; }
    public decimal Weight { get; set; }
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Planned;

    public void StartTransit()
    {
        if (this.Status != ShipmentStatus.Planned)
        {
            throw new Exception($"Cannot transition from {this.Status} to {ShipmentStatus.InTransit}");
        }

        this.Status = ShipmentStatus.InTransit;
    }
    public void MarkAsDelivered()
    {
        if (this.Status != ShipmentStatus.InTransit)
        {
            throw new Exception($"Cannot transition from {this.Status} to {ShipmentStatus.Delivered}");
        }

        this.Status = ShipmentStatus.Delivered;
    }
    public void Cancel()
    {
        if (this.Status == ShipmentStatus.Delivered || this.Status == ShipmentStatus.Cancelled)
        {
            throw new Exception($"Cannot transition from {this.Status} to {ShipmentStatus.Cancelled}");
        }

        this.Status = ShipmentStatus.Cancelled;
    }
}