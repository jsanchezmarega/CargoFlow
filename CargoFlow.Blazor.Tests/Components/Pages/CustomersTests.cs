using System.Net;
using Bunit;
using CargoFlow.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using CustomersPage = CargoFlow.Blazor.Components.Pages.Customers;

namespace CargoFlow.Blazor.Tests.Components.Pages;

public class CustomersTests : BunitContext
{
    [Fact]
    public void LoadsCustomersOnInitialization()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
                [
                    { "id": 1, "name": "ACME" },
                    { "id": 2, "name": "Globex" }
                ]
                """);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        // Act
        var cut = Render<CustomersPage>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll("tbody tr");

            Assert.Equal(2, rows.Count);
            Assert.Contains("ACME", rows[0].TextContent);
            Assert.Contains("Globex", rows[1].TextContent);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void DisplaysErrorWhenLoadingCustomersFails()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(HttpStatusCode.InternalServerError);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<CustomersPage>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Failed to retrieve customers. Please try again later.",
                cut.Markup);

            Assert.DoesNotContain("Loading customers...", cut.Markup);
            Assert.Empty(cut.FindAll("table"));
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void ReloadsCustomersAfterCreation()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """[{"id":1,"name":"ACME"}]""");

        mockHttp.Expect(HttpMethod.Post, "http://localhost/api/customers")
            .WithJsonContent(new { name = "Globex" })
            .Respond(
                HttpStatusCode.Created,
                "application/json",
                """{"id":2,"name":"Globex"}""");

        mockHttp.Expect(HttpMethod.Get, "http://localhost/api/customers")
            .Respond(
                HttpStatusCode.OK,
                "application/json",
                """
            [
                {"id":1,"name":"ACME"},
                {"id":2,"name":"Globex"}
            ]
            """);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<CustomersPage>();

        cut.WaitForAssertion(() =>
        {
            Assert.Single(cut.FindAll("tbody tr"));
            Assert.Contains("ACME", cut.Markup);
        });

        cut.Find("#name").Change("Globex");
        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll("tbody tr");

            Assert.Equal(2, rows.Count);
            Assert.Contains("ACME", rows[0].TextContent);
            Assert.Contains("Globex", rows[1].TextContent);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }
}
