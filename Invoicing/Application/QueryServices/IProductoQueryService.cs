// Invoicing/Application/QueryServices/IProductoQueryService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;

namespace FacturasIA.Platform.Invoicing.Application.QueryServices;

public record PrecioHistorico(DateTime FechaEmision, decimal PrecioUnitario);

public interface IProductoQueryService
{
    Task<IEnumerable<Producto>> Handle(ListarProductosPorProveedorQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<PrecioHistorico>> Handle(ObtenerHistorialPreciosQuery query, CancellationToken cancellationToken);
}