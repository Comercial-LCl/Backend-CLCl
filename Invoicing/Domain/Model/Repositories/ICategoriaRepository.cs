// Invoicing/Domain/Repositories/ICategoriaRepository.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Domain.Repositories;

public interface ICategoriaRepository : IBaseRepository<Categoria>
{
    Task<Categoria?> FindByNombreAsync(string nombre, CancellationToken cancellationToken);
}