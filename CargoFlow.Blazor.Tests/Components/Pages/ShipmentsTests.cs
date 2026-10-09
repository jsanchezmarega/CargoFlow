using System.Net;
using Bunit;
using CargoFlow.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using ShipmentsPage = CargoFlow.Blazor.Components.Pages.Shipments;

namespace CargoFlow.Blazor.Tests.Components.Pages;

public class ShipmentsTests : BunitContext
{
    [Fact]
    public void LoadsShipmentsAndCustomersOnInitialization()
    {
        var mockHttp = new MockHttpMessageHandler();

        var shipmentsRequest = mockHttp
            .When(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
                [
                    {
                        "id": 1001,
                        "customerId": 1,
                        "customerName": "ACME",
                        "origin": "Cologne",
                        "destination": "Munich",
                        "weight": 850,
                        "status": "Planned"
                    }
                ]
                """);

        var customersRequest = mockHttp
            .When(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll("tbody tr");

            Assert.Single(rows);
            Assert.Contains("1001", rows[0].TextContent);
            Assert.Contains("ACME", rows[0].TextContent);
            Assert.Contains("Cologne", rows[0].TextContent);
            Assert.Contains("Munich", rows[0].TextContent);

            var customerOptions = cut.FindAll("#customerId option");

            Assert.Contains(customerOptions,
                option => option.TextContent == "ACME");
        });

        Assert.Equal(1, mockHttp.GetMatchCount(shipmentsRequest));
        Assert.Equal(1, mockHttp.GetMatchCount(customersRequest));
    }

    [Fact]
    public void AppliesFiltersToShipmentRequest()
    {
        var mockHttp = new MockHttpMessageHandler();

        var initialRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/shipments")
            .WithExactQueryString("")
            .Respond(HttpStatusCode.OK, "application/json", "[]");

        var statusRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/shipments")
            .WithExactQueryString("status=Planned")
            .Respond(HttpStatusCode.OK, "application/json", "[]");

        var customerRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/shipments")
            .WithExactQueryString("status=Planned&customerId=1")
            .Respond(HttpStatusCode.OK, "application/json", "[]");

        var originRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/shipments")
            .WithExactQueryString("status=Planned&customerId=1&origin=Cologne")
            .Respond(HttpStatusCode.OK, "application/json", "[]");

        var customersRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, mockHttp.GetMatchCount(initialRequest));
            Assert.Equal(1, mockHttp.GetMatchCount(customersRequest));

            Assert.False(cut.Find("#statusFilter").HasAttribute("disabled"));
            Assert.False(cut.Find("#customerFilter").HasAttribute("disabled"));
        });
        cut.Find("#statusFilter").Change("Planned");

        cut.WaitForAssertion(() =>
            Assert.Equal(1, mockHttp.GetMatchCount(statusRequest)));

        cut.Find("#customerFilter").Change("1");

        cut.WaitForAssertion(() =>
            Assert.Equal(1, mockHttp.GetMatchCount(customerRequest)));

        cut.Find("#originFilter").Change("  Cologne  ");

        cut.WaitForAssertion(() =>
            Assert.Equal(1, mockHttp.GetMatchCount(originRequest)));
    }

    [Fact]
    public void ReloadsShipmentsAfterCreation()
    {
        var mockHttp = new MockHttpMessageHandler(BackendDefinitionBehavior.Always);

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(HttpStatusCode.OK, "application/json", "[]");

        mockHttp.When(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        mockHttp.Expect(HttpMethod.Post, "http://localhost/api/shipments")
            .WithJsonContent(new
            {
                customerId = 1,
                originCity = "Cologne",
                destinationCity = "Munich",
                weight = 850m
            })
            .Respond(
                HttpStatusCode.Created,
                "application/json",
                """
            {
                "id": 1001,
                "customerId": 1,
                "customerName": "ACME",
                "origin": "Cologne",
                "destination": "Munich",
                "weight": 850,
                "status": "Planned"
            }
            """);

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
            [
                {
                    "id": 1001,
                    "customerId": 1,
                    "customerName": "ACME",
                    "origin": "Cologne",
                    "destination": "Munich",
                    "weight": 850,
                    "status": "Planned"
                }
            ]
            """);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("No shipments found.", cut.Markup);
            Assert.False(cut.Find("button[type='submit']").HasAttribute("disabled"));
        });

        cut.Find("#customerId").Change("1");
        cut.Find("#origin").Change("Cologne");
        cut.Find("#destination").Change("Munich");
        cut.Find("#weight").Change("850");

        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll("tbody tr");

            Assert.Single(rows);
            Assert.Contains("1001", rows[0].TextContent);
            Assert.Contains("ACME", rows[0].TextContent);
            Assert.Contains("Cologne", rows[0].TextContent);
            Assert.Contains("Munich", rows[0].TextContent);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void ChangesShipmentStatusAndReloadsList()
    {
        var mockHttp = new MockHttpMessageHandler(BackendDefinitionBehavior.Always);

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
            [{
                "id": 1001,
                "customerId": 1,
                "customerName": "ACME",
                "origin": "Cologne",
                "destination": "Munich",
                "weight": 850,
                "status": "Planned"
            }]
            """);

        mockHttp.When(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        mockHttp.Expect(
                HttpMethod.Post,
                "http://localhost/api/shipments/1001/start-transit")
            .Respond(HttpStatusCode.NoContent);

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
            [{
                "id": 1001,
                "customerId": 1,
                "customerName": "ACME",
                "origin": "Cologne",
                "destination": "Munich",
                "weight": 850,
                "status": "InTransit"
            }]
            """);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            var row = Assert.Single(cut.FindAll("tbody tr"));
            Assert.Contains("Planned", row.TextContent);
        });

        cut.Find("tbody tr button").Click();

        cut.WaitForAssertion(() =>
        {
            var row = Assert.Single(cut.FindAll("tbody tr"));

            Assert.Contains("InTransit", row.TextContent);
            Assert.DoesNotContain("Planned", row.TextContent);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void DisplaysErrorWhenStatusChangeConflicts()
    {
        var mockHttp = new MockHttpMessageHandler(BackendDefinitionBehavior.Always);

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/shipments")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
            [{
                "id": 1001,
                "customerId": 1,
                "customerName": "ACME",
                "origin": "Cologne",
                "destination": "Munich",
                "weight": 850,
                "status": "Planned"
            }]
            """);

        mockHttp.When(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        mockHttp.Expect(
                HttpMethod.Post,
                "http://localhost/api/shipments/1001/start-transit")
            .Respond(HttpStatusCode.Conflict);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            var row = Assert.Single(cut.FindAll("tbody tr"));
            Assert.Contains("Planned", row.TextContent);
        });

        cut.Find("tbody tr button").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "This shipment cannot perform that action in its current state.",
                cut.Markup);

            var row = Assert.Single(cut.FindAll("tbody tr"));
            Assert.Contains("Planned", row.TextContent);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void DisplaysErrorWhenLoadingShipmentsFails()
    {
        var mockHttp = new MockHttpMessageHandler();

        var shipmentsRequest = mockHttp.When(
                HttpMethod.Get,
                "http://localhost/api/shipments")
            .Respond(HttpStatusCode.InternalServerError);

        mockHttp.When(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));
        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<ShipmentsPage>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Failed to retrieve shipments: Please try again later.",
                cut.Markup);

            Assert.DoesNotContain("Loading shipments...", cut.Markup);

            Assert.Empty(cut.FindAll("tbody tr"));
        });

        Assert.Equal(1, mockHttp.GetMatchCount(shipmentsRequest));
    }
}
