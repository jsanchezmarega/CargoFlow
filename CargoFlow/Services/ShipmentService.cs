namespace CargoFlow.Services;

using CargoFlow.Data;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

public class ShipmentService
{
    private readonly CargoFlowDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public ShipmentService(CargoFlowDbContext dbContext, INotificationService notificationService)
    {
        this._dbContext = dbContext;
        this._notificationService = notificationService;
    }

    public async Task<Shipment> CreateShipmentAsync(Customer customer, string originCity,  string destinationCity, decimal weight)
    {
        Shipment shipment = new Shipment(
            customer,
            new Address("Germany", originCity),
            new Address("Germany", destinationCity),
            weight
            );

        _dbContext.Shipments.Add( shipment );
        await _dbContext.SaveChangesAsync();

        return shipment;
    }

    public async Task<Shipment?> CreateShipmentAsync(
        int customerId,
        string originCity,
        string destinationCity,
        decimal weight)
    {
        var customer = await _dbContext.Customers.FindAsync(customerId);

        if (customer is null)
        {
            return null;
        }

        return await CreateShipmentAsync(
            customer,
            originCity,
            destinationCity,
            weight
        );
    }

    public async Task<List<Shipment>> GetShipmentsAsync()
    {
        return await _dbContext.Shipments.Include(s => s.Customer).ToListAsync();
    }

    public async Task<Shipment?> GetShipmentByIdAsync(int id)
    {
        return await _dbContext.Shipments
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task SetInTransitAsync(Shipment shipment)
    {
        shipment.StartTransit();
        await _dbContext.SaveChangesAsync();

        this._notificationService.NotifyShipment(shipment);
    }

    public async Task SetDeliveredAsync(Shipment shipment)
    {
        shipment.MarkAsDelivered();
        await _dbContext.SaveChangesAsync();

        this._notificationService.NotifyShipment(shipment);
    }

    public async Task SetCancelledAsync(Shipment shipment)
    {
        shipment.Cancel();
        await _dbContext.SaveChangesAsync();

        this._notificationService.NotifyShipment(shipment);
    }
}
