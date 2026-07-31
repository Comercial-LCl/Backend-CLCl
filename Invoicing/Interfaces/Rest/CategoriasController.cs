// Invoicing/Interfaces/Rest/CategoriasController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Model.Queries;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/categorias")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Catálogo de categorías de gasto (solo lectura).")]
public class CategoriasController(ICategoriaQueryService categoriaQueryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Listar categorías", "Lista todas las categorías del catálogo global.")]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var categorias = await categoriaQueryService.Handle(new ListarCategoriasQuery(), cancellationToken);
        return Ok(categorias.Select(CategoriaResourceFromEntityAssembler.ToResourceFromEntity));
    }
}