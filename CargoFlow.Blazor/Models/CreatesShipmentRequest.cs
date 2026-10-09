namespace CargoFlow.Blazor.Models;

public record CreateShipmentRequest(
    int CustomerId,
    string OriginCity,
    string DestinationCity,
    decimal Weight
);
