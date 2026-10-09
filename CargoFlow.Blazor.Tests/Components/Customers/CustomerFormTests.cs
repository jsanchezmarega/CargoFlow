using Bunit;
using CargoFlow.Blazor.Components.Customers;
using CargoFlow.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net;

namespace CargoFlow.Blazor.Tests.Components.Customers;

public class CustomerFormTests : BunitContext
{
    [Fact]
    public void DisplaysValidationErrorWhenNameIsEmpty()
    {
        var mockHttp = new MockHttpMessageHandler();
        var unexpectedRequest = mockHttp.When("*");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        var cut = Render<CustomerForm>();

        cut.Find("button[type='submit']").Click();

        Assert.Contains("Customer name is required.", cut.Markup);
        Assert.Equal(0, mockHttp.GetMatchCount(unexpectedRequest));
    }

    [Fact]
    public void CreatesCustomerAndInvokesCallback()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(
                HttpMethod.Post,
                "http://localhost/api/customers")
            .WithJsonContent(new { name = "ACME" })
            .Respond(
                 HttpStatusCode.Created,
                "application/json",
                """{"id":1,"name":"ACME"}""");

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        var callbackInvoked = false;

        var cut = Render<CustomerForm>(parameters => parameters
            .Add(p => p.OnCustomerCreated, () => callbackInvoked = true));

        cut.Find("input#name").Change("ACME");
        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.True(callbackInvoked);
            Assert.Equal("", cut.Find("input#name").GetAttribute("value"));
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void DisplaysErrorWhenCustomerCreationFails()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Post, "http://localhost/api/customers")
            .WithJsonContent(new { name = "ACME" })
            .Respond(HttpStatusCode.InternalServerError);

        using var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        Services.AddSingleton(new CustomerApiClient(httpClient));

        var callbackInvoked = false;

        var cut = Render<CustomerForm>(parameters => parameters
            .Add(p => p.OnCustomerCreated, () => callbackInvoked = true));

        cut.Find("input#name").Change("ACME");
        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Failed to create customer. Please try again.",
                cut.Markup);

            Assert.Equal("ACME", cut.Find("input#name").GetAttribute("value"));
            Assert.False(callbackInvoked);
        });

        mockHttp.VerifyNoOutstandingExpectation();
    }
}