// Invoicing/Interfaces/Rest/ProveedoresController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/proveedores")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Catálogo de proveedores (solo lectura).")]
public class ProveedoresController(
    IProveedorQueryService proveedorQueryService,
    IConsultaRucService consultaRucService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Listar proveedores", "Lista todos los proveedores del catálogo global.")]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var proveedores = await proveedorQueryService.Handle(new ListarProveedoresQuery(), cancellationToken);
        return Ok(proveedores.Select(ProveedorResourceFromEntityAssembler.ToResourceFromEntity));
    }
    [HttpGet("consultar-ruc/{ruc}")]
    [SwaggerOperation("Consultar RUC", "Consulta los datos de una empresa por RUC (SUNAT o proxy equivalente), sin crear el proveedor todavía.")]
    public async Task<IActionResult> ConsultarRuc(string ruc, CancellationToken cancellationToken)
    {
        var datos = await consultaRucService.ConsultarAsync(ruc, cancellationToken);
        return datos is null ? NotFound() : Ok(datos);
    }
}