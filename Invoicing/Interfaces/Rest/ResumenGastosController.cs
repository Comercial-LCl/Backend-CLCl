// Invoicing/Interfaces/Rest/ResumenGastosController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;
using FacturasIA.Platform.Shared.Interfaces.Rest;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/resumen-gastos")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Análisis y resumen de gastos.")]
public class ResumenGastosController(IResumenGastosQueryService resumenGastosQueryService) : ControllerBase
{
    [HttpGet("por-categoria")]
    [SwaggerOperation("Resumen por categoría", "Total de gastos agrupado por categoría.")]
    public async Task<IActionResult> PorCategoria(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var query = new ObtenerResumenGastosPorCategoriaQuery(this.CurrentUserId(), desde, hasta);
        var resumen = await resumenGastosQueryService.Handle(query, cancellationToken);
        return Ok(resumen.Select(ResumenGastosResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("por-periodo")]
    [SwaggerOperation("Resumen por periodo", "Total de gastos en un rango de fechas.")]
    public async Task<IActionResult> PorPeriodo(
        [FromQuery] DateTime desde, [FromQuery] DateTime hasta, CancellationToken cancellationToken)
    {
        var query = new ObtenerResumenGastosPorPeriodoQuery(this.CurrentUserId(), desde, hasta);
        var total = await resumenGastosQueryService.Handle(query, cancellationToken);
        return Ok(new ResumenGastosPorPeriodoResource(total));
    }
}