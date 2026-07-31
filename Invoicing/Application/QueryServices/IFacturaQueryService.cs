// Invoicing/Application/QueryServices/IFacturaQueryService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;

namespace FacturasIA.Platform.Invoicing.Application.QueryServices;

public interface IFacturaQueryService
{
    Task<IEnumerable<Factura>> Handle(ListarFacturasQuery query, CancellationToken cancellationToken);
    Task<Factura?> Handle(ObtenerFacturaPorIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<Factura>> Handle(FiltrarFacturasQuery query, CancellationToken cancellationToken);
}