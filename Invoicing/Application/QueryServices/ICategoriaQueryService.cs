// Invoicing/Application/QueryServices/ICategoriaQueryService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;

namespace FacturasIA.Platform.Invoicing.Application.QueryServices;

public interface ICategoriaQueryService
{
    Task<IEnumerable<Categoria>> Handle(ListarCategoriasQuery query, CancellationToken cancellationToken);
}