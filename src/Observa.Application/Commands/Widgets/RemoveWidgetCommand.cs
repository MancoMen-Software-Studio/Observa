using System;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;

namespace Observa.Application.Commands.Widgets;

/// <summary>
/// Comando para remover un widget de un dashboard.
/// </summary>
public sealed record RemoveWidgetCommand(Guid DashboardId, Guid WidgetId) : ICommand;

/// <summary>
/// Handler que procesa la remocion de un widget de un dashboard.
/// </summary>
public sealed class RemoveWidgetCommandHandler : ICommandHandler<RemoveWidgetCommand>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWidgetCommandHandler(IDashboardRepository dashboardRepository, IUnitOfWork unitOfWork)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveWidgetCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result.Failure(DashboardErrors.NotFound);
        }

        var result = dashboard.RemoveWidget(request.WidgetId);

        if (result.IsFailure)
        {
            return result;
        }

        _dashboardRepository.Update(dashboard);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
