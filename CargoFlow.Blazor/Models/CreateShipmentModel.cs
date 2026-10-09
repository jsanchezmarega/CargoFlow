using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Blazor.Models;

public class CreateShipmentModel : IValidatableObject
{
    [Required(ErrorMessage = "Please select a customer.")]
    public int? CustomerId { get; set; }
    [Required(ErrorMessage = "Origin is required.")]
    public string OriginCity { get; set; } = "";
    [Required(ErrorMessage = "Destination is required.")]
    public string DestinationCity { get; set; } = "";
    [Required(ErrorMessage = "Weight is required.")]
    public decimal? Weight { get; set; }

    public IEnumerable<ValidationResult> Validate(
    ValidationContext validationContext)
    {
        if (Weight <= 0)
        {
            yield return new ValidationResult(
                "Weight must be greater than zero.",
                [nameof(Weight)]
            );
        }
    }

}
