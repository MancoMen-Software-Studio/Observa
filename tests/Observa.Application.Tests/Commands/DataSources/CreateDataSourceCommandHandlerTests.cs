using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Commands.DataSources;
using Observa.Domain.Abstractions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Commands.DataSources;

public sealed class CreateDataSourceCommandHandlerTests
{
    private readonly IDataSourceRepository _dataSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateDataSourceCommandHandler _handler;

    public CreateDataSourceCommandHandlerTests()
    {
        _dataSourceRepository = Substitute.For<IDataSourceRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateDataSourceCommandHandler(_dataSourceRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccessWithId()
    {
        var command = new CreateDataSourceCommand("API Produccion", DataSourceType.RestApi, "https://api.example.com");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCallAddAndSave()
    {
        var command = new CreateDataSourceCommand("API Test", DataSourceType.Database, "Server=localhost");

        await _handler.Handle(command, CancellationToken.None);

        await _dataSourceRepository.Received(1).AddAsync(Arg.Any<DataSource>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldReturnFailure()
    {
        var command = new CreateDataSourceCommand("", DataSourceType.RestApi, "https://api.example.com");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DataSource.EmptyName");
    }

    [Fact]
    public async Task Handle_WithEmptyConnectionString_ShouldReturnFailure()
    {
        var command = new CreateDataSourceCommand("Test", DataSourceType.Database, "");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DataSource.EmptyConnectionString");
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldNotCallRepository()
    {
        var command = new CreateDataSourceCommand("", DataSourceType.RestApi, "https://api.example.com");

        await _handler.Handle(command, CancellationToken.None);

        await _dataSourceRepository.DidNotReceive().AddAsync(Arg.Any<DataSource>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
