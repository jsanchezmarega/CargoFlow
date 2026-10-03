using CargoFlow.Services;
using CargoFlow.Tests.Helpers;
using CargoFlow.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Tests.Services;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_ShouldPersistCustomer()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customerService = new CustomerService(dbContext);

        var customer = await customerService.CreateCustomerAsync(
            "ACME GmbH",
            TestContext.Current.CancellationToken
        );

        await using var verificationContext = database.CreateContext();

        var savedCustomer = await verificationContext.Customers
            .SingleAsync(
                c => c.Id == customer.Id,
                TestContext.Current.CancellationToken
            );

        Assert.Equal("ACME GmbH", savedCustomer.Name);
    }

    [Fact]
    public async Task GetCustomersAsync_ShouldReturnCustomers()
    {
        await using var database = await TestDatabase.CreateAsync(
            TestContext.Current.CancellationToken
        );

        await using var dbContext = database.CreateContext();

        var customerService = new CustomerService(dbContext);

        await customerService.CreateCustomerAsync(
            "ACME GmbH",
            TestContext.Current.CancellationToken
        );

        await customerService.CreateCustomerAsync(
            "Globex AG",
            TestContext.Current.CancellationToken
        );

        var customers = await customerService.GetCustomersAsync(
            TestContext.Current.CancellationToken
        );

        Assert.Equal(2, customers.Count);
        Assert.Contains(customers, customer => customer.Name == "ACME GmbH");
        Assert.Contains(customers, customer => customer.Name == "Globex AG");
    }
}