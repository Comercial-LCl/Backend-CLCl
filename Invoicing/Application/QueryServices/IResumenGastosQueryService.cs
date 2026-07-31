// Invoicing/Application/QueryServices/IResumenGastosQueryService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;

namespace FacturasIA.Platform.Invoicing.Application.QueryServices;

public record ResumenPorCategoria(Guid? CategoriaId, string CategoriaNombre, decimal Total);
public interface IResumenGastosQueryService
{
    Task<IEnumerable<ResumenPorCategoria>> Handle(
        ObtenerResumenGastosPorCategoriaQuery query, CancellationToken cancellationToken);

    Task<decimal> Handle(ObtenerResumenGastosPorPeriodoQuery query, CancellationToken cancellationToken);
}