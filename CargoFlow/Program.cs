using CargoFlow.Data;
using CargoFlow.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContext<CargoFlowDbContext>();
services.AddTransient<ShipmentService>();
services.AddTransient<INotificationService, ConsoleNotificationService>();

var serviceProvider = services.BuildServiceProvider();
