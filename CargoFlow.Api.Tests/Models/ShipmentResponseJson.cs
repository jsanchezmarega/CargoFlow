namespace CargoFlow.Api.Tests.Models;

public record ShipmentResponseJson(
    int Id,
    int CustomerId,
    string CustomerName,
    string Origin,
    string Destination,
    decimal Weight,
    string Status
);