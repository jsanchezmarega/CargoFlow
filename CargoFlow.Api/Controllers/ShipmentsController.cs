using CargoFlow.Api.Dtos.Shipments;
using CargoFlow.Domain;
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

        return Ok(shipments.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShipment(int id)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);

        if (shipment is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(shipment));
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipment(
    CreateShipmentRequest request)
    {
        var shipment = await _shipmentService.CreateShipmentAsync(
            request.CustomerId,
            request.OriginCity,
            request.DestinationCity,
            request.Weight
        );

        if (shipment is null)
        {
            return NotFound();
        }

        var response = ToResponse(shipment);

        return CreatedAtAction(
            nameof(GetShipment),
            new { id = shipment.Id },
            response
        );
    }

    private static ShipmentResponse ToResponse(Shipment shipment)
    {
        return new ShipmentResponse(
            shipment.Id,
            shipment.CustomerId,
            shipment.Customer.Name,
            shipment.Origin.City,
            shipment.Destination.City,
            shipment.Weight,
            shipment.Status.ToString()
        );
    }
}
