using CargoFlow.Api.Tests.Infrastructure;
using CargoFlow.Api.Tests.Models;
using System.Net;
using System.Net.Http.Json;

namespace CargoFlow.Api.Tests.Customers;

public class CustomerEndpointsTests
{
    [Fact]
    public async Task GetCustomer_WhenCustomerExists_ReturnsCustomer()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        var customer = await factory.ExecuteDbAsync(
            dbContext => TestData.CreateCustomerAsync(
                dbContext,
                "API Customer",
                TestContext.Current.CancellationToken));

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/customers/{customer.Id}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseCustomer =
            await response.Content.ReadFromJsonAsync<CustomerResponseJson>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(responseCustomer);
        Assert.Equal(customer.Id, responseCustomer.Id);
        Assert.Equal("API Customer", responseCustomer.Name);
    }

    [Fact]
    public async Task GetCustomer_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/customers/999999",
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_ReturnsCustomers()
    {
        using var factory = new CargoFlowWebApplicationFactory();

        await factory.ExecuteDbAsync(
            async dbContext =>
            {
                await TestData.CreateCustomerAsync(
                    dbContext,
                    "Customer One",
                    TestContext.Current.CancellationToken);

                await TestData.CreateCustomerAsync(
                    dbContext,
                    "Customer Two",
                    TestContext.Current.CancellationToken);
            });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/customers",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var customers =
            await response.Content
                .ReadFromJsonAsync<List<CustomerResponseJson>>(
                    TestContext.Current.CancellationToken);

        Assert.NotNull(customers);
        Assert.Equal(2, customers.Count);

        Assert.Contains(
            customers,
            customer => customer.Name == "Customer One");

        Assert.Contains(
            customers,
            customer => customer.Name == "Customer Two");
    }

    [Fact]
    public async Task CreateCustomer_WithValidRequest_ReturnsCreatedCustomer()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            Name = "Created Customer"
        };

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var customer =
            await response.Content
                .ReadFromJsonAsync<CustomerResponseJson>(
                    TestContext.Current.CancellationToken);

        Assert.NotNull(customer);
        Assert.True(customer.Id > 0);
        Assert.Equal("Created Customer", customer.Name);

        Assert.NotNull(response.Headers.Location);

        Assert.Equal(
            $"/api/Customers/{customer.Id}",
            response.Headers.Location.AbsolutePath);

        var persistedCustomer = await factory.ExecuteDbAsync(
            async dbContext => await dbContext.Customers.FindAsync(
                [customer.Id],
                TestContext.Current.CancellationToken));

        Assert.NotNull(persistedCustomer);
        Assert.Equal("Created Customer", persistedCustomer.Name);
    }

    [Fact]
    public async Task CreateCustomer_WithBlankName_ReturnsBadRequest()
    {
        using var factory = new CargoFlowWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            Name = " "
        };

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}