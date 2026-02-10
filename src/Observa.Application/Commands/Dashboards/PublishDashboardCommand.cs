using System;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.Abstractions.Notifications;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;

namespace Observa.Application.Commands.Dashboards;

/// <summary>
/// Comando para publicar un dashboard en estado borrador.
/// </summary>
public sealed record PublishDashboardCommand(Guid DashboardId) : ICommand;

/// <summary>
/// Handler que procesa la publicacion de un dashboard.
/// </summary>
public sealed class PublishDashboardCommandHandler : ICommandHandler<PublishDashboardCommand>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;

    public PublishDashboardCommandHandler(
        IDashboardRepository dashboardRepository,
        IUnitOfWork unitOfWork,
        IDashboardNotificationService notificationService)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(PublishDashboardCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result.Failure(DashboardErrors.NotFound);
        }

        var result = dashboard.Publish();

        if (result.IsFailure)
        {
            return result;
        }

        _dashboardRepository.Update(dashboard);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifyDashboardUpdatedAsync(request.DashboardId, cancellationToken);
        await _notificationService.NotifyDashboardListChangedAsync(cancellationToken);

        return Result.Success();
    }
}
