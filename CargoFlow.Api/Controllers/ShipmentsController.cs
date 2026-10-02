using CargoFlow.Api.Dtos.Shipments;
using CargoFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly ShipmentService _shipmentService;

    public ShipmentsController(ShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetShipments()
    {
        var shipments = await _shipmentService.GetShipmentsAsync();

        var response = shipments.Select(shipment =>
            new ShipmentResponse(
                shipment.Id,
                shipment.CustomerId,
                shipment.Customer.Name,
                shipment.Origin.City,
                shipment.Destination.City,
                shipment.Weight,
                shipment.Status.ToString()
            )
        );

        return Ok(response);
    }
}
