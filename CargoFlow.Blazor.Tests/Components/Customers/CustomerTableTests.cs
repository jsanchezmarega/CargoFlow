using Bunit;
using CargoFlow.Blazor.Components.Customers;
using CargoFlow.Blazor.Models;

namespace CargoFlow.Blazor.Tests.Components.Customers;

public class CustomerTableTests : BunitContext
{
    [Fact]
    public void RendersCustomers()
    {
        var customers = new[]
        {
            new CustomerResponse(1, "ACME"),
            new CustomerResponse(2, "Globex")
        };

        var cut = Render<CustomerTable>(parameters => parameters
            .Add(p => p.Customers, customers));

        var rows = cut.FindAll("tbody tr");

        Assert.Equal(2, rows.Count);

        Assert.Equal("1", rows[0].QuerySelectorAll("td")[0].TextContent);
        Assert.Equal("ACME", rows[0].QuerySelectorAll("td")[1].TextContent);

        Assert.Equal("2", rows[1].QuerySelectorAll("td")[0].TextContent);
        Assert.Equal("Globex", rows[1].QuerySelectorAll("td")[1].TextContent);
    }

    [Fact]
    public void DisplaysEmptyStateWhenNoCustomersExist()
    {
        var cut = Render<CustomerTable>();

        Assert.Contains("No customers found.", cut.Markup);
        Assert.Empty(cut.FindAll("table"));
    }
}
