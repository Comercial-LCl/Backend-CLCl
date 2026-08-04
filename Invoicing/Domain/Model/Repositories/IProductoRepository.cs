// Invoicing/Domain/Repositories/IProductoRepository.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Domain.Repositories;

public interface IProductoRepository : IBaseRepository<Producto>
{
    Task<Producto?> FindByProveedorAndNombreAsync(Guid proveedorId, string nombre, CancellationToken cancellationToken);

    /// <summary>
    ///     Lista los productos de un proveedor, pero solo los que el usuario autenticado
    ///     ya compró alguna vez (join contra Factura.UsuarioId) — nunca el catálogo completo
    ///     de otros usuarios.
    /// </summary>
    Task<IEnumerable<Producto>> ListByProveedorAndUsuarioAsync(
        Guid proveedorId, Guid usuarioId, CancellationToken cancellationToken);
}