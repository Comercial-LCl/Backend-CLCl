// Invoicing/Domain/Repositories/IFacturaRepository.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Domain.Repositories;

public interface IFacturaRepository : IBaseRepository<Factura>
{
    Task<IEnumerable<Factura>> ListByUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);

    Task<IEnumerable<Factura>> FiltrarAsync(
        Guid usuarioId,
        Guid? proveedorId,
        Guid? categoriaId,
        DateTime? desde,
        DateTime? hasta,
        CancellationToken cancellationToken);

    Task<IEnumerable<(Guid? CategoriaId, decimal Total)>> ResumenPorCategoriaAsync(
        Guid usuarioId,
        DateTime? desde,
        DateTime? hasta,
        CancellationToken cancellationToken);
}