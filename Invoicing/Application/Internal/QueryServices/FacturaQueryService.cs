// Invoicing/Application/Internal/QueryServices/FacturaQueryService.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;

public class FacturaQueryService(IFacturaRepository facturaRepository) : IFacturaQueryService
{
    public async Task<IEnumerable<Factura>> Handle(ListarFacturasQuery query, CancellationToken cancellationToken)
    {
        return await facturaRepository.ListByUsuarioAsync(query.UsuarioId, cancellationToken);
    }

    public async Task<Factura?> Handle(ObtenerFacturaPorIdQuery query, CancellationToken cancellationToken)
    {
        var factura = await facturaRepository.FindByIdAsync(query.FacturaId, cancellationToken);
        return factura is not null && factura.UsuarioId == query.UsuarioId ? factura : null;
    }

    public async Task<IEnumerable<Factura>> Handle(FiltrarFacturasQuery query, CancellationToken cancellationToken)
    {
        return await facturaRepository.FiltrarAsync(
            query.UsuarioId, query.ProveedorId, query.CategoriaId, query.Desde, query.Hasta, cancellationToken);
    }
}