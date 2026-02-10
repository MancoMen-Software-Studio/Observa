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
/// Comando para actualizar el titulo de un dashboard existente.
/// </summary>
public sealed record UpdateDashboardTitleCommand(Guid DashboardId, string NewTitle) : ICommand;

/// <summary>
/// Handler que procesa la actualizacion del titulo de un dashboard.
/// </summary>
public sealed class UpdateDashboardTitleCommandHandler : ICommandHandler<UpdateDashboardTitleCommand>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;

    public UpdateDashboardTitleCommandHandler(
        IDashboardRepository dashboardRepository,
        IUnitOfWork unitOfWork,
        IDashboardNotificationService notificationService)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(UpdateDashboardTitleCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result.Failure(DashboardErrors.NotFound);
        }

        var result = dashboard.UpdateTitle(request.NewTitle);

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
