// Invoicing/Interfaces/Rest/FacturasController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Invoicing.Application.CommandServices;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;
using FacturasIA.Platform.Shared.Interfaces.Rest;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/facturas")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Registro y consulta de facturas.")]
public class FacturasController(
    IFacturaCommandService facturaCommandService,
    IFacturaQueryService facturaQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("fisica")]
    [SwaggerOperation("Registrar factura física", "Registra una factura a partir del QR, con foto opcional.")]
    public async Task<IActionResult> RegistrarFisica(
        [FromForm] RegistrarFacturaFisicaResource resource,
        IFormFile? imagen,
        CancellationToken cancellationToken)
    {
        byte[]? imagenBytes = null;
        string? imagenContentType = null;

        if (imagen is not null)
        {
            await using var stream = new MemoryStream();
            await imagen.CopyToAsync(stream, cancellationToken);
            imagenBytes = stream.ToArray();
            imagenContentType = imagen.ContentType;
        }

        var command = RegistrarFacturaFisicaCommandFromResourceAssembler.ToCommandFromResource(
            resource, this.CurrentUserId(), imagenBytes, imagenContentType);

        var result = await facturaCommandService.Handle(command, cancellationToken);

        return InvoicingActionResultAssembler.ToActionResultFromFacturaResult(
            this, result, problemDetailsFactory,
            factura => Ok(FacturaResourceFromEntityAssembler.ToResourceFromEntity(factura)));
    }

    [HttpPost("electronica")]
    [SwaggerOperation("Registrar factura electrónica", "Registra una factura a partir de un PDF.")]
    public async Task<IActionResult> RegistrarElectronica(IFormFile archivo, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();
        await archivo.CopyToAsync(stream, cancellationToken);

        var command = RegistrarFacturaElectronicaCommandFromResourceAssembler.ToCommandFromResource(
            this.CurrentUserId(), stream.ToArray(), archivo.FileName);

        var result = await facturaCommandService.Handle(command, cancellationToken);

        return InvoicingActionResultAssembler.ToActionResultFromFacturaResult(
            this, result, problemDetailsFactory,
            factura => Ok(FacturaResourceFromEntityAssembler.ToResourceFromEntity(factura)));
    }

    [HttpGet]
    [SwaggerOperation("Listar facturas", "Lista todas las facturas del usuario autenticado.")]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var facturas = await facturaQueryService.Handle(new ListarFacturasQuery(this.CurrentUserId()), cancellationToken);
        return Ok(facturas.Select(FacturaResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Obtener factura", "Obtiene el detalle de una factura por id.")]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken cancellationToken)
    {
        var factura = await facturaQueryService.Handle(new ObtenerFacturaPorIdQuery(id, this.CurrentUserId()), cancellationToken);
        return factura is null ? NotFound() : Ok(FacturaResourceFromEntityAssembler.ToResourceFromEntity(factura));
    }

    [HttpGet("filtrar")]
    [SwaggerOperation("Filtrar facturas", "Filtra facturas por proveedor, categoría y/o rango de fechas.")]
    public async Task<IActionResult> Filtrar(
        [FromQuery] Guid? proveedorId,
        [FromQuery] Guid? categoriaId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        CancellationToken cancellationToken)
    {
        var query = new FiltrarFacturasQuery(this.CurrentUserId(), proveedorId, categoriaId, desde, hasta);
        var facturas = await facturaQueryService.Handle(query, cancellationToken);
        return Ok(facturas.Select(FacturaResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPatch("{id:guid}/corregir")]
    [SwaggerOperation("Corregir factura", "Corrige uno o más campos leídos incorrectamente por la IA.")]
    public async Task<IActionResult> Corregir(
        Guid id, CorregirDatoFacturaResource resource, CancellationToken cancellationToken)
    {
        var command = CorregirDatoFacturaCommandFromResourceAssembler.ToCommandFromResource(
            id, this.CurrentUserId(), resource);

        var result = await facturaCommandService.Handle(command, cancellationToken);

        return InvoicingActionResultAssembler.ToActionResultFromFacturaResult(
            this, result, problemDetailsFactory,
            factura => Ok(FacturaResourceFromEntityAssembler.ToResourceFromEntity(factura)));
    }
}