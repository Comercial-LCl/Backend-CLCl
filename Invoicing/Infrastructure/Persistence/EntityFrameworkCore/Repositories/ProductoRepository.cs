// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Repositories/ProductoRepository.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Entities;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProductoRepository(AppDbContext context) : BaseRepository<Producto>(context), IProductoRepository
{
    public async Task<Producto?> FindByProveedorAndNombreAsync(
        Guid proveedorId, string nombre, CancellationToken cancellationToken)
    {
        return await Context.Set<Producto>()
            .FirstOrDefaultAsync(p => p.ProveedorId == proveedorId && p.Nombre == nombre, cancellationToken);
    }

    public async Task<IEnumerable<Producto>> ListByProveedorAndUsuarioAsync(
        Guid proveedorId, Guid usuarioId, CancellationToken cancellationToken)
    {
        var productoIds = Context.Set<ItemFactura>()
            .Join(Context.Set<Factura>(), i => i.FacturaId, f => f.Id, (i, f) => new { i, f })
            .Where(x => x.f.UsuarioId == usuarioId && x.f.ProveedorId == proveedorId)
            .Select(x => x.i.ProductoId)
            .Distinct();

        return await Context.Set<Producto>()
            .Where(p => productoIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}