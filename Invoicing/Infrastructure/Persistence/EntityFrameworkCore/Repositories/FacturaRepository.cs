// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Repositories/FacturaRepository.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class FacturaRepository(AppDbContext context) : BaseRepository<Factura>(context), IFacturaRepository
{
    public async Task<IEnumerable<Factura>> ListByUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        return await Context.Set<Factura>()
            .Include(f => f.Items)
            .Where(f => f.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Factura>> FiltrarAsync(
        Guid usuarioId, Guid? proveedorId, Guid? categoriaId, DateTime? desde, DateTime? hasta,
        CancellationToken cancellationToken)
    {
        var query = Context.Set<Factura>().Include(f => f.Items).Where(f => f.UsuarioId == usuarioId);

        if (proveedorId is not null) query = query.Where(f => f.ProveedorId == proveedorId.Value);
        if (categoriaId is not null) query = query.Where(f => f.CategoriaId == categoriaId.Value);
        if (desde is not null) query = query.Where(f => f.FechaEmision >= desde.Value);
        if (hasta is not null) query = query.Where(f => f.FechaEmision <= hasta.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<(Guid? CategoriaId, decimal Total)>> ResumenPorCategoriaAsync(
        Guid usuarioId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken)
    {
        var query = Context.Set<Factura>().Where(f => f.UsuarioId == usuarioId);

        if (desde is not null) query = query.Where(f => f.FechaEmision >= desde.Value);
        if (hasta is not null) query = query.Where(f => f.FechaEmision <= hasta.Value);

        var agrupado = await query
            .GroupBy(f => f.CategoriaId)
            .Select(g => new { CategoriaId = g.Key, Total = g.Sum(f => f.Monto.Valor) })
            .ToListAsync(cancellationToken);

        return agrupado.Select(x => (x.CategoriaId, x.Total));
    }
}