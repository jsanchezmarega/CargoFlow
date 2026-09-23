using System.ComponentModel;

namespace CargoFlow.Domain;

public class Shipment : INotifyPropertyChanged
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
    public Customer Customer { get; set; } = null!;
    public int CustomerId { get; private set; }
    public Address Origin { get; set; }
    public Address Destination { get; set; }
    public decimal Weight { get; set; }
    private ShipmentStatus status = ShipmentStatus.Planned;
    public ShipmentStatus Status
    {
        get
        {
            return status;
        }

        private set
        {
            if (status == value)
                return;

            status = value;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(Status))
            );
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void StartTransit()
    {
        if (this.Status != ShipmentStatus.Planned)
        {
            throw new InvalidShipmentStateException($"Cannot transition from {this.Status} to {ShipmentStatus.InTransit}");
        }

        this.Status = ShipmentStatus.InTransit;
    }
    public void MarkAsDelivered()
    {
        if (this.Status != ShipmentStatus.InTransit)
        {
            throw new InvalidShipmentStateException($"Cannot transition from {this.Status} to {ShipmentStatus.Delivered}");
        }

        this.Status = ShipmentStatus.Delivered;
    }
    public void Cancel()
    {
        if (this.Status == ShipmentStatus.Delivered || this.Status == ShipmentStatus.Cancelled)
        {
            throw new InvalidShipmentStateException($"Cannot transition from {this.Status} to {ShipmentStatus.Cancelled}");
        }

        this.Status = ShipmentStatus.Cancelled;
    }
}