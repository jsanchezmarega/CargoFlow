namespace CargoFlow.Services;

using CargoFlow.Data;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

public class ShipmentService
{
    private readonly CargoFlowDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public ShipmentService(
        CargoFlowDbContext dbContext,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    public async Task<Shipment> CreateShipmentAsync(
        Customer customer,
        string originCity,
        string destinationCity,
        decimal weight,
        CancellationToken cancellationToken = default)
    {
        Shipment shipment = new Shipment(
            customer,
            new Address("Germany", originCity),
            new Address("Germany", destinationCity),
            weight
        );

        _dbContext.Shipments.Add(shipment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return shipment;
    }

    public async Task<Shipment?> CreateShipmentAsync(
        int customerId,
        string originCity,
        string destinationCity,
        decimal weight,
        CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.FindAsync(
            [customerId],
            cancellationToken);

        if (customer is null)
        {
            return null;
        }

        return await CreateShipmentAsync(
            customer,
            originCity,
            destinationCity,
            weight,
            cancellationToken
        );
    }

    public async Task<List<Shipment>> GetShipmentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shipments
            .Include(s => s.Customer)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shipment?> GetShipmentByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shipments
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);
    }

    public async Task SetInTransitAsync(
        Shipment shipment,
        CancellationToken cancellationToken = default)
    {
        shipment.StartTransit();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _notificationService.NotifyShipment(shipment);
    }

    public async Task SetDeliveredAsync(
        Shipment shipment,
        CancellationToken cancellationToken = default)
    {
        shipment.MarkAsDelivered();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _notificationService.NotifyShipment(shipment);
    }

    public async Task SetCancelledAsync(
        Shipment shipment,
        CancellationToken cancellationToken = default)
    {
        shipment.Cancel();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _notificationService.NotifyShipment(shipment);
    }
}