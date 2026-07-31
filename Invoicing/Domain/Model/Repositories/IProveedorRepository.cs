// Invoicing/Domain/Repositories/IProveedorRepository.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Domain.Repositories;

public interface IProveedorRepository : IBaseRepository<Proveedor>
{
    Task<Proveedor?> FindByRucAsync(string ruc, CancellationToken cancellationToken);
}