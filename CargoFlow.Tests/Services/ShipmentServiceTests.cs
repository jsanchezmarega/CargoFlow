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
            850,
            TestContext.Current.CancellationToken
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

        var shipments = await shipmentService.GetShipmentsAsync(
            null,
            null,
            null,
            TestContext.Current.CancellationToken
        );

        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(shipment.Id, loadedShipment.Id);
        Assert.Equal("ACME GmbH", loadedShipment.Customer.Name);
    }

    [Fact]
    public async Task GetShipmentsAsync_WithStatusFilter_ShouldReturnOnlyMatchingShipments()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var plannedCustomer = new Customer("Planned Customer");
        var inTransitCustomer = new Customer("In Transit Customer");

        var plannedShipment = new Shipment(
            plannedCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Berlin"),
            100m
        );

        var inTransitShipment = new Shipment(
            inTransitCustomer,
            new Address("Germany", "Hamburg"),
            new Address("Germany", "Munich"),
            200m
        );

        inTransitShipment.StartTransit();

        dbContext.Shipments.AddRange(
            plannedShipment,
            inTransitShipment
        );

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        await using var queryContext = database.CreateContext();

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            queryContext,
            notificationService.Object
        );

        var shipments = await shipmentService.GetShipmentsAsync(
            ShipmentStatus.InTransit,
            null,
            null,
            TestContext.Current.CancellationToken
        );

        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(inTransitShipment.Id, loadedShipment.Id);
        Assert.Equal(ShipmentStatus.InTransit, loadedShipment.Status);
    }

    [Fact]
    public async Task GetShipmentsAsync_WithCustomerIdFilter_ShouldReturnOnlyMatchingShipments()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var firstCustomer = new Customer("First Customer");
        var secondCustomer = new Customer("Second Customer");

        var firstShipment = new Shipment(
            firstCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Berlin"),
            100m
        );

        var secondShipment = new Shipment(
            secondCustomer,
            new Address("Germany", "Hamburg"),
            new Address("Germany", "Munich"),
            200m
        );

        dbContext.Shipments.AddRange(
            firstShipment,
            secondShipment
        );

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        await using var queryContext = database.CreateContext();

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            queryContext,
            notificationService.Object
        );

        var shipments = await shipmentService.GetShipmentsAsync(
            null,
            firstCustomer.Id,
            null,
            TestContext.Current.CancellationToken
        );

        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(firstShipment.Id, loadedShipment.Id);
        Assert.Equal(firstCustomer.Id, loadedShipment.CustomerId);
    }

    [Fact]
    public async Task GetShipmentsAsync_WithOriginFilter_ShouldReturnOnlyMatchingShipments()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var cologneCustomer = new Customer("Cologne Customer");
        var hamburgCustomer = new Customer("Hamburg Customer");

        var cologneShipment = new Shipment(
            cologneCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Berlin"),
            100m
        );

        var hamburgShipment = new Shipment(
            hamburgCustomer,
            new Address("Germany", "Hamburg"),
            new Address("Germany", "Munich"),
            200m
        );

        dbContext.Shipments.AddRange(
            cologneShipment,
            hamburgShipment
        );

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        await using var queryContext = database.CreateContext();

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            queryContext,
            notificationService.Object
        );

        var shipments = await shipmentService.GetShipmentsAsync(
            null,
            null,
            "Cologne",
            TestContext.Current.CancellationToken
        );

        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(cologneShipment.Id, loadedShipment.Id);
        Assert.Equal("Cologne", loadedShipment.Origin.City);
    }

    [Fact]
    public async Task GetShipmentsAsync_WithStatusCustomerIdAndOriginFilters_ShouldReturnOnlyMatchingShipment()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var firstCustomer = new Customer("First Customer");
        var secondCustomer = new Customer("Second Customer");

        // Matches customer + origin, but not status.
        var plannedFirstCustomerCologne = new Shipment(
            firstCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Berlin"),
            100m
        );

        // Matches all three filters.
        var inTransitFirstCustomerCologne = new Shipment(
            firstCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Munich"),
            200m
        );

        // Matches status + origin, but not customer.
        var inTransitSecondCustomerCologne = new Shipment(
            secondCustomer,
            new Address("Germany", "Cologne"),
            new Address("Germany", "Berlin"),
            300m
        );

        // Matches status + customer, but not origin.
        var inTransitFirstCustomerHamburg = new Shipment(
            firstCustomer,
            new Address("Germany", "Hamburg"),
            new Address("Germany", "Berlin"),
            400m
        );

        inTransitFirstCustomerCologne.StartTransit();
        inTransitSecondCustomerCologne.StartTransit();
        inTransitFirstCustomerHamburg.StartTransit();

        dbContext.Shipments.AddRange(
            plannedFirstCustomerCologne,
            inTransitFirstCustomerCologne,
            inTransitSecondCustomerCologne,
            inTransitFirstCustomerHamburg
        );

        await dbContext.SaveChangesAsync(
            TestContext.Current.CancellationToken
        );

        await using var queryContext = database.CreateContext();

        var notificationService = new Mock<INotificationService>();
        var shipmentService = new ShipmentService(
            queryContext,
            notificationService.Object
        );

        var shipments = await shipmentService.GetShipmentsAsync(
            ShipmentStatus.InTransit,
            firstCustomer.Id,
            "Cologne",
            TestContext.Current.CancellationToken
        );

        var loadedShipment = Assert.Single(shipments);

        Assert.Equal(
            inTransitFirstCustomerCologne.Id,
            loadedShipment.Id
        );

        Assert.Equal(
            firstCustomer.Id,
            loadedShipment.CustomerId
        );

        Assert.Equal(
            ShipmentStatus.InTransit,
            loadedShipment.Status
        );

        Assert.Equal(
            "Cologne",
            loadedShipment.Origin.City
        );
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

        await shipmentService.SetInTransitAsync(
            shipment,
            TestContext.Current.CancellationToken
        );

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
            () => shipmentService.SetInTransitAsync(
                shipment,
                TestContext.Current.CancellationToken
            )
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

        await shipmentService.SetDeliveredAsync(
            shipment,
            TestContext.Current.CancellationToken
        );

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
            () => shipmentService.SetDeliveredAsync(
                shipment,
                TestContext.Current.CancellationToken
            )
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

        await shipmentService.SetCancelledAsync(
            shipment,
            TestContext.Current.CancellationToken
        );

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
            () => shipmentService.SetCancelledAsync(
                shipment,
                TestContext.Current.CancellationToken
            )
        );

        notificationService.Verify(
            service => service.NotifyShipment(It.IsAny<Shipment>()),
            Times.Never
        );
    }
}