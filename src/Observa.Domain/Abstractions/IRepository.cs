using System;
using System.Threading;
using System.Threading.Tasks;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Interfaz base para repositorios del dominio.
/// </summary>
public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
