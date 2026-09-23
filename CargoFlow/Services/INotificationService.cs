namespace CargoFlow.Services;

using CargoFlow.Domain;

public interface INotificationService
{
    void NotifyShipment(Shipment? shipment);
}
