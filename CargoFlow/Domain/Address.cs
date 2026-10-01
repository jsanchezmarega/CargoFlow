namespace CargoFlow.Domain;

public record Address
{
    private Address()
    {
    }

    public Address(string country, string city)
    {
        ArgumentNullException.ThrowIfNull(country);
        ArgumentNullException.ThrowIfNull(city);

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException(
                "Country cannot be empty.",
                nameof(country)
            );
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "City cannot be empty.",
                nameof(city)
            );
        }

        Country = country;
        City = city;
    }

    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
}