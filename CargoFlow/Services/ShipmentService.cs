namespace CargoFlow.Services;

using CargoFlow.Data;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

class ShipmentService
{
    private CargoFlowDbContext _dbContext;
    private INotificationService _notificationService;

    public ShipmentService(CargoFlowDbContext dbContext, INotificationService notificationService)
    {
        this._dbContext = dbContext;
        this._notificationService = notificationService;
    }

    public void SetInTransit(Shipment shipment)
    {
        shipment.StartTransit();
        this._notificationService.NotifyShipment(shipment);
    }

    public async Task<List<Shipment>> GetPlannedShipmentsAsync()
    {
            var shipments = await this._dbContext
            .Shipments
            .Include(s => s.Customer)
            .Where(s => s.Status == ShipmentStatus.Planned)
            .OrderByDescending(s => s.Weight)
            .ToListAsync();

        return shipments;
    }
}
