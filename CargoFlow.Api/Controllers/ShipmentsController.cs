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
    public async Task<ActionResult<List<ShipmentResponse>>> GetShipments(
        [FromQuery] ShipmentStatus? status,
        [FromQuery] int? customerId,
        [FromQuery] string? origin,
        CancellationToken cancellationToken)
    {
        var shipments = await _shipmentService.GetShipmentsAsync(
            status,
            customerId,
            origin,
            cancellationToken);

        return Ok(shipments.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShipment(
        int id,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(
            id,
            cancellationToken);

        if (shipment is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(shipment));
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipment(
        CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.CreateShipmentAsync(
            request.CustomerId,
            request.OriginCity,
            request.DestinationCity,
            request.Weight,
            cancellationToken
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

    [HttpPost("{id:int}/start-transit")]
        public async Task<ActionResult<ShipmentResponse>> StartTransit(
        int id,
    CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(
            id,
            cancellationToken);


        if (shipment is null)
        {
            return NotFound();
        }

        await _shipmentService.SetInTransitAsync(
            shipment,
            cancellationToken);

        return Ok(ToResponse(shipment));
    }

    [HttpPost("{id:int}/deliver")]
    public async Task<ActionResult<ShipmentResponse>> Deliver(
        int id,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(
            id,
            cancellationToken);

        if (shipment is null)
        {
            return NotFound();
        }

        await _shipmentService.SetDeliveredAsync(
            shipment,
            cancellationToken);

        return Ok(ToResponse(shipment));
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ShipmentResponse>> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(
            id,
            cancellationToken);

        if (shipment is null)
        {
            return NotFound();
        }

        await _shipmentService.SetCancelledAsync(
            shipment,
            cancellationToken);

        return Ok(ToResponse(shipment));
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
