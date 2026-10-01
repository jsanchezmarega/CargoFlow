using System.ComponentModel;

namespace CargoFlow.Domain;

public class Shipment : INotifyPropertyChanged
{
    private Shipment()
    {
    }

    public Shipment(
        Customer customer,
        Address origin,
        Address destination,
        decimal weight)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);

        if (weight <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weight),
                weight,
                "Shipment weight must be greater than zero."
            );
        }

        Customer = customer;
        Origin = origin;
        Destination = destination;
        Weight = weight;
    }

    public int Id { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public int CustomerId { get; private set; }
    public Address Origin { get; private set; } = null!;
    public Address Destination { get; private set; } = null!;
    public decimal Weight { get; private set; }
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