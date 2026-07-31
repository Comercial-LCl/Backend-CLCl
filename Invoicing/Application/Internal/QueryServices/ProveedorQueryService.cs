// Invoicing/Application/Internal/QueryServices/ProveedorQueryService.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;

public class ProveedorQueryService(IProveedorRepository proveedorRepository) : IProveedorQueryService
{
    public async Task<IEnumerable<Proveedor>> Handle(ListarProveedoresQuery query, CancellationToken cancellationToken)
    {
        return await proveedorRepository.ListAsync(cancellationToken);
    }
}