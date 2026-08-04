// Invoicing/Interfaces/Rest/ProductosController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;
using FacturasIA.Platform.Shared.Interfaces.Rest;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Productos por proveedor e historial de precios.")]
public class ProductosController(IProductoQueryService productoQueryService) : ControllerBase
{
    [HttpGet("proveedores/{proveedorId:guid}/productos")]
    [SwaggerOperation("Listar productos por proveedor",
        "Lista los productos que el usuario autenticado ya compró a ese proveedor.")]
    public async Task<IActionResult> ListarPorProveedor(Guid proveedorId, CancellationToken cancellationToken)
    {
        var query = new ListarProductosPorProveedorQuery(proveedorId, this.CurrentUserId());
        var productos = await productoQueryService.Handle(query, cancellationToken);
        return Ok(productos.Select(ProductoResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("productos/{productoId:guid}/historial-precios")]
    [SwaggerOperation("Historial de precios",
        "Lista el precio pagado por ese producto en cada factura del usuario autenticado, ordenado por fecha.")]
    public async Task<IActionResult> HistorialPrecios(Guid productoId, CancellationToken cancellationToken)
    {
        var query = new ObtenerHistorialPreciosQuery(productoId, this.CurrentUserId());
        var historial = await productoQueryService.Handle(query, cancellationToken);
        return Ok(historial.Select(PrecioHistoricoResourceFromEntityAssembler.ToResourceFromEntity));
    }
}