using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CargoFlow.Data;

public class CargoFlowDbContextFactory
    : IDesignTimeDbContextFactory<CargoFlowDbContext>
{
    public CargoFlowDbContext CreateDbContext(string[] args)
    {
        var connectionString = DatabaseConfiguration.GetConnectionString();

        var options = new DbContextOptionsBuilder<CargoFlowDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new CargoFlowDbContext(options);
    }
}
