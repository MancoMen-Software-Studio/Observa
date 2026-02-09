using FluentAssertions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Xunit;

namespace Observa.Domain.Tests.Entities;

public sealed class DataSourceTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        var result = DataSource.Create("API Produccion", DataSourceType.RestApi, "https://api.example.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("API Produccion");
        result.Value.Type.Should().Be(DataSourceType.RestApi);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        var result = DataSource.Create("", DataSourceType.Database, "Server=localhost");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DataSource.EmptyName");
    }

    [Fact]
    public void Create_WithEmptyConnectionString_ShouldReturnFailure()
    {
        var result = DataSource.Create("Test", DataSourceType.Database, "");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DataSource.EmptyConnectionString");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var dataSource = DataSource.Create("Test", DataSourceType.RestApi, "https://api.test.com").Value;

        dataSource.Deactivate();

        dataSource.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_AfterDeactivation_ShouldSetIsActiveTrue()
    {
        var dataSource = DataSource.Create("Test", DataSourceType.RestApi, "https://api.test.com").Value;
        dataSource.Deactivate();

        dataSource.Activate();

        dataSource.IsActive.Should().BeTrue();
    }

    [Fact]
    public void MarkAsSynced_ShouldUpdateLastSyncAt()
    {
        var dataSource = DataSource.Create("Test", DataSourceType.WebSocket, "wss://stream.test.com").Value;

        dataSource.MarkAsSynced();

        dataSource.LastSyncAt.Should().NotBeNull();
    }
}
