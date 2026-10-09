using Bunit;
using CargoFlow.Blazor.Components.Shipments;
using CargoFlow.Blazor.Models;

namespace CargoFlow.Blazor.Tests.Components.Shipments;

public class ShipmentTableTests : BunitContext
{
    [Fact]
    public void RendersShipmentData()
    {
        var shipments = new[]
        {
            new ShipmentResponse(
                1001,
                1,
                "ACME",
                "Cologne",
                "Munich",
                850m,
                "Planned")
        };

        var cut = Render<ShipmentTable>(parameters => parameters
            .Add(p => p.Shipments, shipments));

        var rows = cut.FindAll("tbody tr");

        Assert.Single(rows);

        var cells = cut.FindAll("tbody tr td");

        Assert.Equal("1001", cells[0].TextContent.Trim());
        Assert.Equal("ACME", cells[1].TextContent.Trim());
    }

    [Theory]
    [InlineData("Planned", true, false, true)]
    [InlineData("InTransit", false, true, true)]
    [InlineData("Delivered", false, false, false)]
    [InlineData("Cancelled", false, false, false)]
    public void RendersActionsBasedOnStatus(
        string status,
        bool canStartTransit,
        bool canDeliver,
        bool canCancel)
    {
        var shipments = new[]
        {
            new ShipmentResponse(
                1001, 1, "ACME", "Cologne", "Munich", 850m, status)
        };

        var cut = Render<ShipmentTable>(parameters => parameters
            .Add(p => p.Shipments, shipments));

        var buttons = cut.FindAll("tbody button");
        var buttonLabels = buttons
            .Select(button => button.TextContent.Trim())
            .ToArray();

        Assert.Equal(canStartTransit, buttonLabels.Contains("Start Transit"));
        Assert.Equal(canDeliver, buttonLabels.Contains("Mark Delivered"));
        Assert.Equal(canCancel, buttonLabels.Contains("Cancel"));
    }

    [Theory]
    [InlineData("Planned", "start-transit", "button.btn-primary")]
    [InlineData("InTransit", "deliver", "button.btn-success")]
    [InlineData("Planned", "cancel", "button.btn-outline-danger")]
    [InlineData("InTransit", "cancel", "button.btn-outline-danger")]
    public void ClickingActionInvokesCallbackWithShipmentId(
    string status,
    string action,
    string buttonSelector)
    {
        var shipments = new[]
        {
            new ShipmentResponse(
                1001, 1, "ACME", "Cologne", "Munich", 850m, status)
        };

        int? receivedShipmentId = null;

        var cut = Render<ShipmentTable>(parameters =>
        {
            parameters.Add(p => p.Shipments, shipments);

            switch (action)
            {
                case "start-transit":
                    parameters.Add(p => p.OnStartTransit,
                        (int id) => receivedShipmentId = id);
                    break;

                case "deliver":
                    parameters.Add(p => p.OnDeliver,
                        (int id) => receivedShipmentId = id);
                    break;

                case "cancel":
                    parameters.Add(p => p.OnCancel,
                        (int id) => receivedShipmentId = id);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        });

        cut.Find(buttonSelector).Click();

        Assert.Equal(1001, receivedShipmentId);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(1001, true)]
    public void DisablesActionButtonsWhileProcessing(
    int? processingShipmentId,
    bool shouldBeDisabled)
    {
        var shipments = new[]
        {
            new ShipmentResponse(
                1001, 1, "ACME", "Cologne", "Munich", 850m, "Planned"),
            new ShipmentResponse(
                1002, 1, "ACME", "Berlin", "Hamburg", 500m, "InTransit")
        };

        var cut = Render<ShipmentTable>(parameters => parameters
            .Add(p => p.Shipments, shipments)
            .Add(p => p.ProcessingShipmentId, processingShipmentId));

        var buttons = cut.FindAll("tbody button");

        Assert.Equal(4, buttons.Count);

        Assert.All(buttons, button =>
            Assert.Equal(shouldBeDisabled, button.HasAttribute("disabled")));
    }

    [Fact]
    public void DisplaysEmptyStateWhenNoShipmentsExist()
    {
        var cut = Render<ShipmentTable>(parameters => parameters
            .Add(p => p.Shipments, Array.Empty<ShipmentResponse>()));

        Assert.Contains("No shipments found.", cut.Markup);
        Assert.Empty(cut.FindAll("table"));
    }
}
