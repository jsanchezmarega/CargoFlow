using CargoFlow.Domain;
using CargoFlow.Services;
using CargoFlow.Tests.Helpers;
using CargoFlow.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CargoFlow.Tests.Services;

public class ShipmentServiceTests
{
    [Fact]
    public async Task CreateShipmentAsync_ShouldPersistShipmentWithCustomer()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        dbContext.Customers.Add(customer);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        var shipment = await shipmentService.CreateShipmentAsync(
            customer,
            "Cologne",
            "Munich",
            850
        );

        await using var verificationContext = database.CreateContext();

        var savedShipment = await verificationContext.Shipments
            .Include(s => s.Customer)
            .SingleAsync(
                s => s.Id == shipment.Id,
                TestContext.Current.CancellationToken
            );

        Assert.Equal("Cologne", savedShipment.Origin.City);
        Assert.Equal("Munich", savedShipment.Destination.City);
        Assert.Equal(850, savedShipment.Weight);
        Assert.Equal(customer.Id, savedShipment.CustomerId);
        Assert.Equal("ACME GmbH", savedShipment.Customer.Name);
    }

    [Fact]
    public async Task GetShipmentsAsync_ShouldReturnShipmentsWithCustomers()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        await using var queryContext = database.CreateContext();

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            queryContext,
            notificationService.Object
        );

        var shipments = await shipmentService.GetShipmentsAsync();
        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(shipment.Id, loadedShipment.Id);
        Assert.Equal("ACME GmbH", loadedShipment.Customer.Name);
    }

    [Fact]
    public async Task SetInTransitAsync_ShouldPersistStatusAndNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await shipmentService.SetInTransitAsync(shipment);

        await using var verificationContext = database.CreateContext();

        var savedShipment = await verificationContext.Shipments
            .SingleAsync(
                s => s.Id == shipment.Id,
                TestContext.Current.CancellationToken
            );

        Assert.Equal(ShipmentStatus.InTransit, savedShipment.Status);

        notificationService.Verify(
            service => service.NotifyShipment(shipment),
            Times.Once
        );
    }

    [Fact]
    public async Task SetInTransitAsync_WhenTransitionIsInvalid_ShouldNotNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        shipment.StartTransit();

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await Assert.ThrowsAsync<InvalidShipmentStateException>(
             () => shipmentService.SetInTransitAsync(shipment)
        );

        notificationService.Verify(
            service => service.NotifyShipment(It.IsAny<Shipment>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SetDeliveredAsync_ShouldPersistStatusAndNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);
        shipment.StartTransit();

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await shipmentService.SetDeliveredAsync(shipment);

        await using var verificationContext = database.CreateContext();

        var savedShipment = await verificationContext.Shipments
            .SingleAsync(
                s => s.Id == shipment.Id,
                TestContext.Current.CancellationToken
            );

        Assert.Equal(ShipmentStatus.Delivered, savedShipment.Status);

        notificationService.Verify(
            service => service.NotifyShipment(shipment),
            Times.Once
        );
    }

    [Fact]
    public async Task SetDeliveredAsync_WhenTransitionIsInvalid_ShouldNotNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await Assert.ThrowsAsync<InvalidShipmentStateException>(
             () => shipmentService.SetDeliveredAsync(shipment)
        );

        notificationService.Verify(
            service => service.NotifyShipment(It.IsAny<Shipment>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SetCancelledAsync_ShouldPersistStatusAndNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await shipmentService.SetCancelledAsync(shipment);

        await using var verificationContext = database.CreateContext();

        var savedShipment = await verificationContext.Shipments
            .SingleAsync(
                s => s.Id == shipment.Id,
                TestContext.Current.CancellationToken
            );

        Assert.Equal(ShipmentStatus.Cancelled, savedShipment.Status);

        notificationService.Verify(
            service => service.NotifyShipment(shipment),
            Times.Once
        );
    }

    [Fact]
    public async Task SetCancelledAsync_WhenTransitionIsInvalid_ShouldNotNotify()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);
        shipment.StartTransit();
        shipment.MarkAsDelivered();

        dbContext.Customers.Add(customer);
        dbContext.Shipments.Add(shipment);

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        var notificationService = new Mock<INotificationService>();

        var shipmentService = new ShipmentService(
            dbContext,
            notificationService.Object
        );

        await Assert.ThrowsAsync<InvalidShipmentStateException>(
             () => shipmentService.SetCancelledAsync(shipment)
        );

        notificationService.Verify(
            service => service.NotifyShipment(It.IsAny<Shipment>()),
            Times.Never
        );
    }
}
