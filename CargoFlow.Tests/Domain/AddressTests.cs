using CargoFlow.Domain;

namespace CargoFlow.Tests.Domain;

public class AddressTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WhenCountryIsBlank_ShouldThrow(string country)
    {
        Assert.Throws<ArgumentException>(
            () => new Address(country, "Cologne")
        );
    }

    [Fact]
    public void Constructor_WhenCountryIsNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Address(null!, "Cologne")
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WhenCityIsBlank_ShouldThrow(string city)
    {
        Assert.Throws<ArgumentException>(
            () => new Address("Germany", city)
        );
    }

    [Fact]
    public void Constructor_WhenCityIsNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Address("Germany", null!)
        );
    }
}