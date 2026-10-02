using CargoFlow.Data;
using CargoFlow.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = DatabaseConfiguration.GetConnectionString();

builder.Services.AddDbContext<CargoFlowDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddTransient<ShipmentService>();
builder.Services.AddTransient<CustomerService>();
builder.Services.AddTransient<INotificationService, ConsoleNotificationService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
