using System.Threading;
using System.Threading.Tasks;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Unidad de trabajo para manejar transacciones de persistencia.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
