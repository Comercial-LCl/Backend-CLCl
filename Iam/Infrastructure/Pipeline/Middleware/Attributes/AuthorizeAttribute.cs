// Iam/Infrastructure/Pipeline/Middleware/Attributes/AuthorizeAttribute.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;

namespace FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marca controllers/acciones que requieren autorización. Solo hay un rol de usuario
///     (dueño de negocio), así que a diferencia del zip del profesor no hay chequeo de Roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;

        var usuario = (Usuario?)context.HttpContext.Items["User"];
        if (usuario is null)
            context.Result = new UnauthorizedResult();
    }
}