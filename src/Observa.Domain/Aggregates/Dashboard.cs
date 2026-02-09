using System;
using System.Collections.Generic;
using System.Linq;
using Observa.Domain.Abstractions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Events;
using Observa.Domain.ValueObjects;

namespace Observa.Domain.Aggregates;

/// <summary>
/// Raiz de agregado que representa un dashboard con sus widgets y configuracion.
/// </summary>
public sealed class Dashboard : AggregateRoot
{
    private readonly List<Widget> _widgets = [];
    private readonly List<ThresholdRule> _thresholdRules = [];

    private Dashboard(Guid id, string title, string description)
        : base(id)
    {
        Title = title;
        Description = description;
        Status = DashboardStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Dashboard()
    {
    }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DashboardStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Widget> Widgets => _widgets.AsReadOnly();

    public IReadOnlyCollection<ThresholdRule> ThresholdRules => _thresholdRules.AsReadOnly();

    public static Result<Dashboard> Create(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Dashboard>.Failure(DashboardErrors.EmptyTitle);
        }

        if (title.Length > 200)
        {
            return Result<Dashboard>.Failure(DashboardErrors.TitleTooLong);
        }

        var dashboard = new Dashboard(Guid.NewGuid(), title, description ?? string.Empty);
        dashboard.RaiseDomainEvent(new DashboardCreatedEvent(dashboard.Id, title));

        return Result<Dashboard>.Success(dashboard);
    }

    public Result AddWidget(Widget widget)
    {
        if (_widgets.Count >= 20)
        {
            return Result.Failure(DashboardErrors.MaxWidgetsReached);
        }

        if (_widgets.Any(w => w.Id == widget.Id))
        {
            return Result.Failure(DashboardErrors.DuplicateWidget);
        }

        _widgets.Add(widget);
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new WidgetAddedEvent(Id, widget.Id, widget.Title));

        return Result.Success();
    }

    public Result RemoveWidget(Guid widgetId)
    {
        var widget = _widgets.FirstOrDefault(w => w.Id == widgetId);

        if (widget is null)
        {
            return Result.Failure(WidgetErrors.NotFound);
        }

        _widgets.Remove(widget);
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new WidgetRemovedEvent(Id, widgetId));

        return Result.Success();
    }

    public Result UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return Result.Failure(DashboardErrors.EmptyTitle);
        }

        if (newTitle.Length > 200)
        {
            return Result.Failure(DashboardErrors.TitleTooLong);
        }

        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateDescription(string newDescription)
    {
        Description = newDescription ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Publish()
    {
        if (Status == DashboardStatus.Published)
        {
            return Result.Failure(DashboardErrors.AlreadyPublished);
        }

        if (_widgets.Count == 0)
        {
            return Result.Failure(DashboardErrors.NoWidgets);
        }

        Status = DashboardStatus.Published;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new DashboardPublishedEvent(Id));

        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == DashboardStatus.Archived)
        {
            return Result.Failure(DashboardErrors.AlreadyArchived);
        }

        Status = DashboardStatus.Archived;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void AddThresholdRule(ThresholdRule rule)
    {
        _thresholdRules.Add(rule);
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Errores relacionados con el agregado Dashboard.
/// </summary>
public static class DashboardErrors
{
    public static readonly Error EmptyTitle = new("Dashboard.EmptyTitle", "El titulo del dashboard no puede estar vacio.");
    public static readonly Error TitleTooLong = new("Dashboard.TitleTooLong", "El titulo del dashboard no puede superar 200 caracteres.");
    public static readonly Error NotFound = new("Dashboard.NotFound", "El dashboard no fue encontrado.");
    public static readonly Error MaxWidgetsReached = new("Dashboard.MaxWidgetsReached", "El dashboard no puede tener mas de 20 widgets.");
    public static readonly Error DuplicateWidget = new("Dashboard.DuplicateWidget", "El widget ya existe en el dashboard.");
    public static readonly Error AlreadyPublished = new("Dashboard.AlreadyPublished", "El dashboard ya esta publicado.");
    public static readonly Error AlreadyArchived = new("Dashboard.AlreadyArchived", "El dashboard ya esta archivado.");
    public static readonly Error NoWidgets = new("Dashboard.NoWidgets", "No se puede publicar un dashboard sin widgets.");
}
