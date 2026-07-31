// Iam/Domain/Repositories/IUsuarioRepository.cs
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Iam.Domain.Repositories;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}