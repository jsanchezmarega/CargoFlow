using CargoFlow.Api.Tests.Infrastructure;
using CargoFlow.Api.Tests.Models;
using System.Net;
using System.Net.Http.Json;

namespace CargoFlow.Api.Tests.Shipments;

public class ShipmentEndpointsTests
{
    [Fact]
    public async Task GetShipment_WhenShipmentDoesNotExist_ReturnsNotFound()
    {
        using var factory =
            new CargoFlowWebApplicationFactory();

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/shipments/999999",
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetShipment_WhenShipmentExists_ReturnsShipment()
    {
        using var factory =
            new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                cancellationToken:
                    TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/shipments/{shipment.Id}",
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var responseShipment =
            await response.Content
                .ReadFromJsonAsync<ShipmentResponseJson>(
                    TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipment);

        Assert.Equal(shipment.Id, responseShipment.Id);
        Assert.Equal(shipment.CustomerId, responseShipment.CustomerId);
        Assert.Equal("Test Customer", responseShipment.CustomerName);
        Assert.Equal("Cologne", responseShipment.Origin);
        Assert.Equal("Berlin", responseShipment.Destination);
        Assert.Equal(125.5m, responseShipment.Weight);
        Assert.Equal("Planned", responseShipment.Status);
    }

    [Fact]
    public async Task CreateShipment_WithValidRequest_ReturnsCreatedShipment()
    {
        using var factory =
            new CargoFlowWebApplicationFactory();

        var customer = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateCustomerAsync(
                dbContext,
                "API Customer",
                TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var request = new
        {
            CustomerId = customer.Id,
            OriginCity = "Hamburg",
            DestinationCity = "Munich",
            Weight = 500m
        };

        var response = await client.PostAsJsonAsync(
            "/api/shipments",
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var shipment =
            await response.Content
                .ReadFromJsonAsync<ShipmentResponseJson>(
                    TestContext.Current.CancellationToken);

        Assert.NotNull(shipment);

        Assert.Equal(customer.Id, shipment.CustomerId);
        Assert.Equal("API Customer", shipment.CustomerName);
        Assert.Equal("Hamburg", shipment.Origin);
        Assert.Equal("Munich", shipment.Destination);
        Assert.Equal(500m, shipment.Weight);
        Assert.Equal("Planned", shipment.Status);

        Assert.NotNull(response.Headers.Location);

        Assert.Equal(
            $"/api/Shipments/{shipment.Id}",
            response.Headers.Location.AbsolutePath);
    }
}