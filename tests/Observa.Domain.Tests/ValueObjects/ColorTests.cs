using FluentAssertions;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.ValueObjects;

public sealed class ColorTests
{
    [Theory]
    [InlineData("#FF0000")]
    [InlineData("#00ff00")]
    [InlineData("#0000FF")]
    [InlineData("#AbCdEf")]
    public void Create_WithValidHex_ShouldReturnSuccess(string hex)
    {
        var result = Color.Create(hex);

        result.IsSuccess.Should().BeTrue();
        result.Value.HexValue.Should().Be(hex.ToUpperInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ShouldReturnFailure(string? hex)
    {
        var result = Color.Create(hex!);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Color.Empty");
    }

    [Theory]
    [InlineData("FF0000")]
    [InlineData("#FFF")]
    [InlineData("#GGGGGG")]
    [InlineData("#FF00")]
    public void Create_WithInvalidFormat_ShouldReturnFailure(string hex)
    {
        var result = Color.Create(hex);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Color.InvalidFormat");
    }

    [Fact]
    public void Equality_SameHex_ShouldBeEqual()
    {
        var color1 = Color.Create("#FF0000").Value;
        var color2 = Color.Create("#ff0000").Value;

        color1.Should().Be(color2);
    }
}
