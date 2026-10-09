using Bunit;
using CargoFlow.Blazor.Components.Shipments;
using CargoFlow.Blazor.Models;
using CargoFlow.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net;

namespace CargoFlow.Blazor.Tests.Components.Shipments;

public class ShipmentFormTests : BunitContext
{
    [Fact]
    public void DisplaysValidationErrorsWhenFieldsAreEmpty()
    {
        var mockHttp = new MockHttpMessageHandler();
        var unexpectedRequest = mockHttp.When("*");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));

        var customers = new[]
        {
            new CustomerResponse(1, "ACME")
        };

        var cut = Render<ShipmentForm>(parameters => parameters
            .Add(p => p.Customers, customers));

        cut.Find("button[type='submit']").Click();

        Assert.Contains("Please select a customer.", cut.Markup);
        Assert.Contains("Origin is required.", cut.Markup);
        Assert.Contains("Destination is required.", cut.Markup);
        Assert.Contains("Weight is required.", cut.Markup);

        Assert.Equal(0, mockHttp.GetMatchCount(unexpectedRequest));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void RejectsNonPositiveWeight(int weight)
    {
        var mockHttp = new MockHttpMessageHandler();
        var unexpectedRequest = mockHttp.When("*");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));

        var customers = new[]
        {
        new CustomerResponse(1, "ACME")
    };

        var cut = Render<ShipmentForm>(parameters => parameters
            .Add(p => p.Customers, customers));

        cut.Find("#customerId").Change("1");
        cut.Find("#origin").Change("Cologne");
        cut.Find("#destination").Change("Munich");
        cut.Find("#weight").Change(weight.ToString());

        cut.Find("button[type='submit']").Click();

        Assert.Contains("Weight must be greater than zero.", cut.Markup);
        Assert.Equal(0, mockHttp.GetMatchCount(unexpectedRequest));
    }

    [Fact]
    public void CreatesShipmentAndInvokesCallback()
    {
        var mockHttp = new MockHttpMessageHandler();

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

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));

        var customers = new[]
        {
            new CustomerResponse(1, "ACME")
        };

        var callbackInvoked = false;

        var cut = Render<ShipmentForm>(parameters => parameters
            .Add(p => p.Customers, customers)
            .Add(p => p.OnShipmentCreated, () => callbackInvoked = true));

        cut.Find("#customerId").Change("1");
        cut.Find("#origin").Change("  Cologne  ");
        cut.Find("#destination").Change("  Munich  ");
        cut.Find("#weight").Change("850");

        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.True(callbackInvoked);

            Assert.True(string.IsNullOrEmpty(
                cut.Find("#origin").GetAttribute("value")));

            Assert.True(string.IsNullOrEmpty(
                cut.Find("#destination").GetAttribute("value")));

            Assert.True(string.IsNullOrEmpty(
                cut.Find("#weight").GetAttribute("value")));
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void DisplaysErrorWhenShipmentCreationFails()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Post, "http://localhost/api/shipments")
            .WithJsonContent(new
            {
                customerId = 1,
                originCity = "Cologne",
                destinationCity = "Munich",
                weight = 850m
            })
            .Respond(HttpStatusCode.InternalServerError);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new ShipmentApiClient(httpClient));

        var customers = new[]
        {
            new CustomerResponse(1, "ACME")
        };

        var callbackInvoked = false;

        var cut = Render<ShipmentForm>(parameters => parameters
            .Add(p => p.Customers, customers)
            .Add(p => p.OnShipmentCreated, () => callbackInvoked = true));

        cut.Find("#customerId").Change("1");
        cut.Find("#origin").Change("Cologne");
        cut.Find("#destination").Change("Munich");
        cut.Find("#weight").Change("850");

        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Failed to create shipment. Please try again.",
                cut.Markup);

            Assert.Equal("Cologne", cut.Find("#origin").GetAttribute("value"));
            Assert.Equal("Munich", cut.Find("#destination").GetAttribute("value"));
            Assert.Equal("850", cut.Find("#weight").GetAttribute("value"));

            Assert.False(callbackInvoked);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }
}
