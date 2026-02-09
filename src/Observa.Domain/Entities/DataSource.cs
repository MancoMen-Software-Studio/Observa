using System;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;

namespace Observa.Domain.Entities;

/// <summary>
/// Origen de datos que alimenta a los widgets del dashboard.
/// </summary>
public sealed class DataSource : Entity
{
    private DataSource(Guid id, string name, DataSourceType type, string connectionString)
        : base(id)
    {
        Name = name;
        Type = type;
        ConnectionString = connectionString;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private DataSource()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public DataSourceType Type { get; private set; }

    public string ConnectionString { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? LastSyncAt { get; private set; }

    public static Result<DataSource> Create(string name, DataSourceType type, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<DataSource>.Failure(DataSourceErrors.EmptyName);
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Result<DataSource>.Failure(DataSourceErrors.EmptyConnectionString);
        }

        return Result<DataSource>.Success(new DataSource(Guid.NewGuid(), name, type, connectionString));
    }

    public void MarkAsSynced()
    {
        LastSyncAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}

/// <summary>
/// Errores relacionados con la entidad DataSource.
/// </summary>
public static class DataSourceErrors
{
    public static readonly Error EmptyName = new("DataSource.EmptyName", "El nombre del origen de datos no puede estar vacio.");
    public static readonly Error EmptyConnectionString = new("DataSource.EmptyConnectionString", "La cadena de conexion no puede estar vacia.");
    public static readonly Error NotFound = new("DataSource.NotFound", "El origen de datos no fue encontrado.");
}
