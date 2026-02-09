using FluentAssertions;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.ValueObjects;

public sealed class WidgetPositionTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        var result = WidgetPosition.Create(0, 0, 4, 3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Column.Should().Be(0);
        result.Value.Row.Should().Be(0);
        result.Value.Width.Should().Be(4);
        result.Value.Height.Should().Be(3);
    }

    [Fact]
    public void Create_WithNegativePosition_ShouldReturnFailure()
    {
        var result = WidgetPosition.Create(-1, 0, 4, 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WidgetPosition.NegativePosition");
    }

    [Fact]
    public void Create_WithZeroWidth_ShouldReturnFailure()
    {
        var result = WidgetPosition.Create(0, 0, 0, 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WidgetPosition.InvalidDimensions");
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        var pos1 = WidgetPosition.Create(1, 2, 3, 4).Value;
        var pos2 = WidgetPosition.Create(1, 2, 3, 4).Value;

        pos1.Should().Be(pos2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        var pos1 = WidgetPosition.Create(1, 2, 3, 4).Value;
        var pos2 = WidgetPosition.Create(5, 6, 7, 8).Value;

        pos1.Should().NotBe(pos2);
    }
}
