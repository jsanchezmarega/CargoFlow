using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Blazor.Models;

public class CreateCustomerModel
{
    [Required(ErrorMessage = "Customer name is required.")]
    public string Name { get; set; } = "";
}
