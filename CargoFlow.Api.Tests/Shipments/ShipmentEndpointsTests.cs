using CargoFlow.Api.Tests.Infrastructure;
using CargoFlow.Api.Tests.Models;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
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
    public async Task GetShipments_ReturnsAllShipments()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var firstShipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                customerName: "First Customer",
                originCity: "Cologne",
                destinationCity: "Berlin",
                cancellationToken:
                    TestContext.Current.CancellationToken));

        var secondShipment = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateShipmentAsync(
                dbContext,
                customerName: "Second Customer",
                originCity: "Hamburg",
                destinationCity: "Munich",
                cancellationToken:
                    TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/shipments",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var shipments =
            await response.Content.ReadFromJsonAsync<List<ShipmentResponseJson>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(shipments);
        Assert.Equal(2, shipments.Count);

        Assert.Contains(
            shipments,
            shipment => shipment.Id == firstShipment.Id);

        Assert.Contains(
            shipments,
            shipment => shipment.Id == secondShipment.Id);
    }

    [Fact]
    public async Task GetShipments_WithStatusFilter_ReturnsOnlyMatchingShipments()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipments = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var plannedShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "Planned Customer",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                var inTransitShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "In Transit Customer",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                inTransitShipment.StartTransit();

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return new
                {
                    plannedShipment = plannedShipment,
                    inTransitShipment = inTransitShipment
                };
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/shipments?status=InTransit",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipments =
            await response.Content.ReadFromJsonAsync<List<ShipmentResponseJson>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipments);

        var responseShipment = Assert.Single(responseShipments);

        Assert.Equal(
            shipments.inTransitShipment.Id,
            responseShipment.Id);

        Assert.Equal(
            "InTransit",
            responseShipment.Status);
    }

    [Fact]
    public async Task GetShipments_WithInvalidStatus_ReturnsBadRequest()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/shipments?status=Invalid",
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task GetShipments_WithCustomerIdFilter_ReturnsOnlyMatchingShipments()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipments = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var shipment1 =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "Customer 1",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                var shipment2 =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "Customer 2",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return new
                {
                    shipment1 = shipment1,
                    shipment2 = shipment2
                };
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/shipments?customerId={shipments.shipment1.CustomerId}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipments =
            await response.Content.ReadFromJsonAsync<List<ShipmentResponseJson>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipments);

        var responseShipment = Assert.Single(responseShipments);

        Assert.Equal(
            shipments.shipment1.Id,
            responseShipment.Id);

        Assert.Equal(
            shipments.shipment1.Customer.Id,
            responseShipment.CustomerId);
    }

    [Fact]
    public async Task GetShipments_WithOriginFilter_ReturnsOnlyMatchingShipments()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipments = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var cologneShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "Cologne Customer",
                        originCity: "Cologne",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                var hamburgShipment =
                    await TestData.CreateShipmentAsync(
                        dbContext,
                        customerName: "Hamburg Customer",
                        originCity: "Hamburg",
                        cancellationToken:
                            TestContext.Current.CancellationToken);

                return new
                {
                    CologneShipment = cologneShipment,
                    HamburgShipment = hamburgShipment
                };
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/shipments?origin=Cologne",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipments =
            await response.Content.ReadFromJsonAsync<List<ShipmentResponseJson>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipments);

        var responseShipment = Assert.Single(responseShipments);

        Assert.Equal(
            shipments.CologneShipment.Id,
            responseShipment.Id);

        Assert.Equal(
            "Cologne",
            responseShipment.Origin);
    }

    [Fact]
    public async Task GetShipments_WithStatusCustomerIdAndOriginFilters_ReturnsOnlyMatchingShipment()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var shipments = await factory.ExecuteDbAsync(
            async dbContext =>
            {
                var firstCustomer = new Customer("First Customer");
                var secondCustomer = new Customer("Second Customer");

                // Matches customer + origin, but not status.
                var plannedFirstCustomerCologne = new Shipment(
                    firstCustomer,
                    new Address("Germany", "Cologne"),
                    new Address("Germany", "Berlin"),
                    100m);

                // Matches all three filters.
                var inTransitFirstCustomerCologne = new Shipment(
                    firstCustomer,
                    new Address("Germany", "Cologne"),
                    new Address("Germany", "Munich"),
                    200m);

                // Matches status + origin, but not customer.
                var inTransitSecondCustomerCologne = new Shipment(
                    secondCustomer,
                    new Address("Germany", "Cologne"),
                    new Address("Germany", "Berlin"),
                    300m);

                // Matches status + customer, but not origin.
                var inTransitFirstCustomerHamburg = new Shipment(
                    firstCustomer,
                    new Address("Germany", "Hamburg"),
                    new Address("Germany", "Berlin"),
                    400m);

                inTransitFirstCustomerCologne.StartTransit();
                inTransitSecondCustomerCologne.StartTransit();
                inTransitFirstCustomerHamburg.StartTransit();

                dbContext.Shipments.AddRange(
                    plannedFirstCustomerCologne,
                    inTransitFirstCustomerCologne,
                    inTransitSecondCustomerCologne,
                    inTransitFirstCustomerHamburg);

                await dbContext.SaveChangesAsync(
                    TestContext.Current.CancellationToken);

                return new
                {
                    ExpectedShipment = inTransitFirstCustomerCologne,
                    CustomerId = firstCustomer.Id
                };
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/shipments?status=InTransit&customerId={shipments.CustomerId}&origin=Cologne",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseShipments =
            await response.Content.ReadFromJsonAsync<List<ShipmentResponseJson>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseShipments);

        var responseShipment = Assert.Single(responseShipments);

        Assert.Equal(
            shipments.ExpectedShipment.Id,
            responseShipment.Id);

        Assert.Equal(
            shipments.CustomerId,
            responseShipment.CustomerId);

        Assert.Equal(
            "InTransit",
            responseShipment.Status);

        Assert.Equal(
            "Cologne",
            responseShipment.Origin);
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
