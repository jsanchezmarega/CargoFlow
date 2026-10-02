namespace CargoFlow.Api.Dtos.Shipments;

public record ShipmentResponse(
    int Id,
    int CustomerId,
    string CustomerName,
    string Origin,
    string Destination,
    decimal Weight,
    string Status
);