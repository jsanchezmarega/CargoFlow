using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Api.Dtos.Customers;

public record CreateCustomerRequest(
    [Required]
    string Name
);
