using CargoFlow.Data;
using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Services;

public class CustomerService
{
    private readonly CargoFlowDbContext _dbContext;

    public CustomerService(CargoFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer> CreateCustomerAsync(string name)
    {
        Customer customer = new Customer(name);

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return customer;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _dbContext.Customers.ToListAsync();
    }
}
