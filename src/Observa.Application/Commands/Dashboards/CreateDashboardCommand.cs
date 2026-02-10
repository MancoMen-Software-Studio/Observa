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
/// Comando para crear un nuevo dashboard.
/// </summary>
public sealed record CreateDashboardCommand(string Title, string Description) : ICommand<Guid>;

/// <summary>
/// Handler que procesa la creacion de un dashboard.
/// </summary>
public sealed class CreateDashboardCommandHandler : ICommandHandler<CreateDashboardCommand, Guid>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;

    public CreateDashboardCommandHandler(
        IDashboardRepository dashboardRepository,
        IUnitOfWork unitOfWork,
        IDashboardNotificationService notificationService)
    {
        _dashboardRepository = dashboardRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(CreateDashboardCommand request, CancellationToken cancellationToken)
    {
        var result = Dashboard.Create(request.Title, request.Description);

        if (result.IsFailure)
        {
            return Result<Guid>.Failure(result.Error);
        }

        await _dashboardRepository.AddAsync(result.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifyDashboardListChangedAsync(cancellationToken);

        return Result<Guid>.Success(result.Value.Id);
    }
}
