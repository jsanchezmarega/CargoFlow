namespace CargoFlow.Services;

using CargoFlow.Domain;

interface INotificationService
{
    void NotifyShipment(Shipment? shipment);
}
