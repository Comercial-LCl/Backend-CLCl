// Invoicing/Application/Internal/QueryServices/ProductoQueryService.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;

public class ProductoQueryService(
    IProductoRepository productoRepository,
    IFacturaRepository facturaRepository)
    : IProductoQueryService
{
    public async Task<IEnumerable<Producto>> Handle(
        ListarProductosPorProveedorQuery query, CancellationToken cancellationToken)
    {
        return await productoRepository.ListByProveedorAndUsuarioAsync(
            query.ProveedorId, query.UsuarioId, cancellationToken);
    }

    public async Task<IEnumerable<PrecioHistorico>> Handle(
        ObtenerHistorialPreciosQuery query, CancellationToken cancellationToken)
    {
        var historial = await facturaRepository.HistorialPreciosPorProductoAsync(
            query.ProductoId, query.UsuarioId, cancellationToken);

        return historial
            .Select(h => new PrecioHistorico(h.FechaEmision, h.PrecioUnitario))
            .OrderBy(h => h.FechaEmision);
    }
}