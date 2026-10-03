using CargoFlow.Api.Dtos.Customers;
using CargoFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetCustomersAsync(
            cancellationToken);

        var response = customers.Select(customer =>
            new CustomerResponse(
                customer.Id,
                customer.Name
            )
        );

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(
        int id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        var response = new CustomerResponse(
            customer.Id,
            customer.Name
        );

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateCustomerAsync(
            request.Name,
            cancellationToken);

        var response = new CustomerResponse(
            customer.Id,
            customer.Name
        );

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = customer.Id },
            response
        );
    }
}