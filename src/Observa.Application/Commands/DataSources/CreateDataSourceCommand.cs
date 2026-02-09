using System;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Domain.Abstractions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;

namespace Observa.Application.Commands.DataSources;

/// <summary>
/// Comando para crear un nuevo origen de datos.
/// </summary>
public sealed record CreateDataSourceCommand(
    string Name,
    DataSourceType Type,
    string ConnectionString) : ICommand<Guid>;

/// <summary>
/// Handler que procesa la creacion de un origen de datos.
/// </summary>
public sealed class CreateDataSourceCommandHandler : ICommandHandler<CreateDataSourceCommand, Guid>
{
    private readonly IDataSourceRepository _dataSourceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDataSourceCommandHandler(IDataSourceRepository dataSourceRepository, IUnitOfWork unitOfWork)
    {
        _dataSourceRepository = dataSourceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateDataSourceCommand request, CancellationToken cancellationToken)
    {
        var result = DataSource.Create(request.Name, request.Type, request.ConnectionString);

        if (result.IsFailure)
        {
            return Result<Guid>.Failure(result.Error);
        }

        await _dataSourceRepository.AddAsync(result.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value.Id);
    }
}
