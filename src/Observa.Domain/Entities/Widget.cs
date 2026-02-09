using System;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;
using Observa.Domain.ValueObjects;

namespace Observa.Domain.Entities;

/// <summary>
/// Representa un widget de visualizacion dentro de un dashboard.
/// </summary>
public sealed class Widget : Entity
{
    private Widget(
        Guid id,
        string title,
        WidgetType type,
        WidgetPosition position,
        Guid dataSourceId,
        RefreshInterval refreshInterval)
        : base(id)
    {
        Title = title;
        Type = type;
        Position = position;
        DataSourceId = dataSourceId;
        RefreshInterval = refreshInterval;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Widget()
    {
    }

    public string Title { get; private set; } = string.Empty;

    public WidgetType Type { get; private set; }

    public WidgetPosition Position { get; private set; } = null!;

    public Guid DataSourceId { get; private set; }

    public RefreshInterval RefreshInterval { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<Widget> Create(
        string title,
        WidgetType type,
        WidgetPosition position,
        Guid dataSourceId,
        RefreshInterval refreshInterval)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Widget>.Failure(WidgetErrors.EmptyTitle);
        }

        if (title.Length > 100)
        {
            return Result<Widget>.Failure(WidgetErrors.TitleTooLong);
        }

        if (dataSourceId == Guid.Empty)
        {
            return Result<Widget>.Failure(WidgetErrors.InvalidDataSource);
        }

        var widget = new Widget(Guid.NewGuid(), title, type, position, dataSourceId, refreshInterval);

        return Result<Widget>.Success(widget);
    }

    public Result UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return Result.Failure(WidgetErrors.EmptyTitle);
        }

        if (newTitle.Length > 100)
        {
            return Result.Failure(WidgetErrors.TitleTooLong);
        }

        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void UpdatePosition(WidgetPosition newPosition)
    {
        Position = newPosition;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRefreshInterval(RefreshInterval interval)
    {
        RefreshInterval = interval;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Errores relacionados con la entidad Widget.
/// </summary>
public static class WidgetErrors
{
    public static readonly Error EmptyTitle = new("Widget.EmptyTitle", "El titulo del widget no puede estar vacio.");
    public static readonly Error TitleTooLong = new("Widget.TitleTooLong", "El titulo del widget no puede superar 100 caracteres.");
    public static readonly Error InvalidDataSource = new("Widget.InvalidDataSource", "El origen de datos del widget no es valido.");
    public static readonly Error NotFound = new("Widget.NotFound", "El widget no fue encontrado.");
}
