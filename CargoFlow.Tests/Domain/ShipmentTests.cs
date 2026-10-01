using CargoFlow.Domain;
using CargoFlow.Tests.Helpers;

namespace CargoFlow.Tests.Domain;

public class ShipmentTests
{
    [Fact]
    public void NewShipment_ShouldHavePlannedStatus()
    {
        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        Assert.Equal(ShipmentStatus.Planned, shipment.Status);
    }

    [Fact]
    public void Shipment_ShouldStartTransit()
    {
        var shipment = CreateShipmentInStatus(ShipmentStatus.Planned);

        shipment.StartTransit();

        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);
    }

    [Fact]
    public void Shipment_ShouldDeliver()
    {
        var shipment = CreateShipmentInStatus(ShipmentStatus.InTransit);

        shipment.MarkAsDelivered();

        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
    }

    [Theory]
    [InlineData(ShipmentStatus.Planned)]
    [InlineData(ShipmentStatus.InTransit)]
    public void Shipment_ShouldCancel(ShipmentStatus startingStatus)
    {
        var shipment = CreateShipmentInStatus(startingStatus);

        shipment.Cancel();

        Assert.Equal(ShipmentStatus.Cancelled, shipment.Status);
    }

    [Theory]
    [InlineData(ShipmentStatus.InTransit)]
    [InlineData(ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled)]
    public void Shipment_ShouldNotStartTransitFromInvalidState(ShipmentStatus startingStatus)
    {
        var shipment = CreateShipmentInStatus(startingStatus);

        Assert.Throws<InvalidShipmentStateException>(
            () => shipment.StartTransit()
        );
    }

    [Theory]
    [InlineData(ShipmentStatus.Planned)]
    [InlineData(ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled)]
    public void Shipment_ShouldNotDeliverFromInvalidState(ShipmentStatus startingStatus)
    {
        var shipment = CreateShipmentInStatus(startingStatus);

        Assert.Throws<InvalidShipmentStateException>(
            () => shipment.MarkAsDelivered()
        );
    }

    [Theory]
    [InlineData(ShipmentStatus.Delivered)]
    [InlineData(ShipmentStatus.Cancelled)]
    public void Shipment_ShouldNotCancelFromInvalidState(ShipmentStatus startingStatus)
    {
        var shipment = CreateShipmentInStatus(startingStatus);

        Assert.Throws<InvalidShipmentStateException>(
            () => shipment.Cancel()
        );
    }

    [Fact]
    public void StartTransit_ShouldRaiseStatusPropertyChanged()
    {
        var shipment = CreateShipmentInStatus(ShipmentStatus.Planned);
        string? changedProperty = null;

        shipment.PropertyChanged += (_, args) =>
        {
            changedProperty = args.PropertyName;
        };

        shipment.StartTransit();

        Assert.Equal(nameof(Shipment.Status), changedProperty);
    }

    [Fact]
    public void Constructor_WhenCustomerIsNull_ShouldThrow()
    {
        var origin = new Address("Germany", "Cologne");
        var destination = new Address("Germany", "Munich");

        Assert.Throws<ArgumentNullException>(
            () => new Shipment(null!, origin, destination, 500)
        );
    }

    [Fact]
    public void Constructor_WhenOriginIsNull_ShouldThrow()
    {
        var customer = new Customer("ACME GmbH");
        var destination = new Address("Germany", "Munich");

        Assert.Throws<ArgumentNullException>(
            () => new Shipment(customer, null!, destination, 500)
        );
    }

    [Fact]
    public void Constructor_WhenDestinationIsNull_ShouldThrow()
    {
        var customer = new Customer("ACME GmbH");
        var origin = new Address("Germany", "Cologne");

        Assert.Throws<ArgumentNullException>(
            () => new Shipment(customer, origin, null!, 500)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public void Constructor_WhenWeightIsNotPositive_ShouldThrow(decimal weight)
    {
        var customer = new Customer("ACME GmbH");
        var origin = new Address("Germany", "Cologne");
        var destination = new Address("Germany", "Munich");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Shipment(customer, origin, destination, weight)
        );
    }

    private static Shipment CreateShipmentInStatus(ShipmentStatus status)
    {
        var customer = TestData.CreateCustomer();
        var shipment = TestData.CreateShipment(customer);

        switch (status)
        {
            case ShipmentStatus.InTransit:
                shipment.StartTransit();
                break;

            case ShipmentStatus.Delivered:
                shipment.StartTransit();
                shipment.MarkAsDelivered();
                break;

            case ShipmentStatus.Cancelled:
                shipment.Cancel();
                break;
        }

        return shipment;
    }
}
