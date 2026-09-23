namespace CargoFlow.Services;

using CargoFlow.Domain;

public class ConsoleNotificationService : INotificationService
{
    public void NotifyShipment(Shipment? shipment)
    {
        if (shipment is not null && shipment.Status == ShipmentStatus.InTransit)
        {
            Console.WriteLine($"Sending notification to {shipment.Customer} about shipment {shipment.Id}");
        }
    }
}
