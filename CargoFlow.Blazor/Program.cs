using CargoFlow.Blazor.Components;
using CargoFlow.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var cargoFlowApiBaseUrl = builder.Configuration["CargoFlowApi:BaseUrl"]
    ?? throw new InvalidOperationException("CargoFlowApi:BaseUrl is not configured.");

builder.Services.AddHttpClient<ShipmentApiClient>(client =>
{
    client.BaseAddress = new Uri(cargoFlowApiBaseUrl);
});

builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(cargoFlowApiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
