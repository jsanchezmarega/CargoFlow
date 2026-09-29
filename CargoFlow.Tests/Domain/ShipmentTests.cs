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
