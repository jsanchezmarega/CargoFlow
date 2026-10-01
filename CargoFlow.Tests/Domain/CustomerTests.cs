using CargoFlow.Domain;

namespace CargoFlow.Tests.Domain;

public class CustomerTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsBlank_ShouldThrow(string name)
    {
        Assert.Throws<ArgumentException>(
            () => new Customer(name)
        );
    }

    [Fact]
    public void Constructor_WhenNameIsNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Customer(null!)
        );
    }
}
