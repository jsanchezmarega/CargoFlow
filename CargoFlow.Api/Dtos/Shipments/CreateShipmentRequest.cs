using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Api.Dtos.Shipments;

public record CreateShipmentRequest(
    [Range(1, int.MaxValue)]
    int CustomerId,

    [Required]
    string OriginCity,

    [Required]
    string DestinationCity,

    decimal Weight
) : IValidatableObject
{
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
