// Invoicing/Application/QueryServices/IProveedorQueryService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;

namespace FacturasIA.Platform.Invoicing.Application.QueryServices;

public interface IProveedorQueryService
{
    Task<IEnumerable<Proveedor>> Handle(ListarProveedoresQuery query, CancellationToken cancellationToken);
}