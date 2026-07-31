// Invoicing/Application/Internal/QueryServices/ResumenGastosQueryService.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Domain.Repositories;

namespace FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;

public class ResumenGastosQueryService(
    IFacturaRepository facturaRepository,
    ICategoriaRepository categoriaRepository)
    : IResumenGastosQueryService
{
    public async Task<IEnumerable<ResumenPorCategoria>> Handle(
        ObtenerResumenGastosPorCategoriaQuery query, CancellationToken cancellationToken)
    {
        var totales = await facturaRepository.ResumenPorCategoriaAsync(
            query.UsuarioId, query.Desde, query.Hasta, cancellationToken);

        var resultado = new List<ResumenPorCategoria>();
        foreach (var (categoriaId, total) in totales)
        {
            if (categoriaId is null)
            {
                resultado.Add(new ResumenPorCategoria(null, "Sin categoría", total));
                continue;
            }

            var categoria = await categoriaRepository.FindByIdAsync(categoriaId.Value, cancellationToken);
            resultado.Add(new ResumenPorCategoria(categoriaId, categoria?.Nombre ?? "Sin categoría", total));
        }

        return resultado;
    }

    public async Task<decimal> Handle(
        ObtenerResumenGastosPorPeriodoQuery query, CancellationToken cancellationToken)
    {
        var facturas = await facturaRepository.FiltrarAsync(
            query.UsuarioId, null, null, query.Desde, query.Hasta, cancellationToken);

        return facturas.Sum(f => f.Monto.Valor);
    }
}