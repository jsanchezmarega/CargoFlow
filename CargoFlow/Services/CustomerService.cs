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

    public async Task<Customer> CreateCustomerAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        Customer customer = new Customer(name);

        _dbContext.Customers.Add(customer);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public async Task<List<Customer>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetCustomerByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.FindAsync(
            [id],
            cancellationToken);
    }
}
