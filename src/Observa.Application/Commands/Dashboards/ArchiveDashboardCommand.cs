using System;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;

namespace Observa.Application.Commands.Dashboards;

/// <summary>
/// Comando para archivar un dashboard.
/// </summary>
public sealed record ArchiveDashboardCommand(Guid DashboardId) : ICommand;

/// <summary>
/// Handler que procesa el archivado de un dashboard.
/// </summary>
public sealed class ArchiveDashboardCommandHandler : ICommandHandler<ArchiveDashboardCommand>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveDashboardCommandHandler(IDashboardRepository dashboardRepository, IUnitOfWork unitOfWork)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveDashboardCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result.Failure(DashboardErrors.NotFound);
        }

        var result = dashboard.Archive();

        if (result.IsFailure)
        {
            return result;
        }

        _dashboardRepository.Update(dashboard);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
