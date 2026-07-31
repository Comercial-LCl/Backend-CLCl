// Invoicing/Application/Internal/QueryServices/CategoriaQueryService.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;

public class CategoriaQueryService(ICategoriaRepository categoriaRepository) : ICategoriaQueryService
{
    public async Task<IEnumerable<Categoria>> Handle(ListarCategoriasQuery query, CancellationToken cancellationToken)
    {
        return await categoriaRepository.ListAsync(cancellationToken);
    }
}