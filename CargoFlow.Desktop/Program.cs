using Microsoft.Extensions.DependencyInjection;
using CargoFlow.Services;
using CargoFlow.Data;

namespace CargoFlow.Desktop;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        var services = new ServiceCollection();

        services.AddDbContext<CargoFlowDbContext>();
        services.AddTransient<ShipmentService>();
        services.AddTransient<INotificationService, ConsoleNotificationService>();
        services.AddTransient<Form1>();

        var serviceProvider = services.BuildServiceProvider();

        ApplicationConfiguration.Initialize();
        Application.Run(serviceProvider.GetRequiredService<Form1>());
    }    
}