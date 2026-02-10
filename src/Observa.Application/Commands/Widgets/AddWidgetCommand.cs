using System;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.Abstractions.Notifications;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Observa.Domain.ValueObjects;

namespace Observa.Application.Commands.Widgets;

/// <summary>
/// Comando para agregar un widget a un dashboard.
/// </summary>
public sealed record AddWidgetCommand(
    Guid DashboardId,
    string Title,
    WidgetType Type,
    int Column,
    int Row,
    int Width,
    int Height,
    Guid DataSourceId,
    RefreshInterval RefreshInterval) : ICommand<Guid>;

/// <summary>
/// Handler que procesa la adicion de un widget a un dashboard.
/// </summary>
public sealed class AddWidgetCommandHandler : ICommandHandler<AddWidgetCommand, Guid>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;

    public AddWidgetCommandHandler(
        IDashboardRepository dashboardRepository,
        IUnitOfWork unitOfWork,
        IDashboardNotificationService notificationService)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(AddWidgetCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result<Guid>.Failure(DashboardErrors.NotFound);
        }

        var positionResult = WidgetPosition.Create(request.Column, request.Row, request.Width, request.Height);

        if (positionResult.IsFailure)
        {
            return Result<Guid>.Failure(positionResult.Error);
        }

        var widgetResult = Widget.Create(
            request.Title,
            request.Type,
            positionResult.Value,
            request.DataSourceId,
            request.RefreshInterval);

        if (widgetResult.IsFailure)
        {
            return Result<Guid>.Failure(widgetResult.Error);
        }

        var addResult = dashboard.AddWidget(widgetResult.Value);

        if (addResult.IsFailure)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        _dashboardRepository.Update(dashboard);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifyWidgetAddedAsync(
            request.DashboardId, widgetResult.Value.Id, cancellationToken);

        return Result<Guid>.Success(widgetResult.Value.Id);
    }
}
