using CargoFlow.Api.Tests.Infrastructure;
using CargoFlow.Api.Tests.Models;
using CargoFlow.Domain;
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

        var persistedShipment = await factory.ExecuteDbAsync(
            async dbContext => await dbContext.Shipments.FindAsync(
                [shipment.Id],
                TestContext.Current.CancellationToken));

        Assert.NotNull(persistedShipment);
        Assert.Equal(customer.Id, persistedShipment.CustomerId);
        Assert.Equal(500m, persistedShipment.Weight);
        Assert.Equal("Hamburg", persistedShipment.Origin.City);
        Assert.Equal("Munich", persistedShipment.Destination.City);
        Assert.Equal(ShipmentStatus.Planned, persistedShipment.Status);
    }

    [Fact]
    public async Task StartTransit_WhenShipmentIsPlanned_ReturnsUpdatedShipment()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                cancellationToken:
                    TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/start-transit",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipment =
            await response.Content.ReadFromJsonAsync<ShipmentResponseJson>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipment);
        Assert.Equal(shipment.Id, responseShipment.Id);
        Assert.Equal("InTransit", responseShipment.Status);
    }

    [Fact]
    public async Task StartTransit_WhenShipmentDoesNotExist_ReturnsNotFound()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            "/api/shipments/999999/start-transit",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StartTransit_WhenShipmentIsAlreadyInTransit_ReturnsConflict()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var createdShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                createdShipment.StartTransit();

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return createdShipment;
            });

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/start-transit",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Deliver_WhenShipmentIsInTransit_ReturnsUpdatedShipment()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var createdShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                createdShipment.StartTransit();

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return createdShipment;
            });

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/deliver",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipment =
            await response.Content.ReadFromJsonAsync<ShipmentResponseJson>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipment);
        Assert.Equal(shipment.Id, responseShipment.Id);
        Assert.Equal("Delivered", responseShipment.Status);
    }

    [Fact]
    public async Task Deliver_WhenShipmentDoesNotExist_ReturnsNotFound()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            "/api/shipments/999999/deliver",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Deliver_WhenShipmentIsPlanned_ReturnsConflict()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                cancellationToken:
                    TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/deliver",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Cancel_WhenShipmentIsPlanned_ReturnsUpdatedShipment()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                cancellationToken:
                    TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/cancel",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipment =
            await response.Content.ReadFromJsonAsync<ShipmentResponseJson>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipment);
        Assert.Equal(shipment.Id, responseShipment.Id);
        Assert.Equal("Cancelled", responseShipment.Status);
    }

    [Fact]
    public async Task Cancel_WhenShipmentDoesNotExist_ReturnsNotFound()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            "/api/shipments/999999/cancel",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Cancel_WhenShipmentIsDelivered_ReturnsConflict()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipment = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var createdShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                createdShipment.StartTransit();
                createdShipment.MarkAsDelivered();

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return createdShipment;
            });

        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/shipments/{shipment.Id}/cancel",
            content: null,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
