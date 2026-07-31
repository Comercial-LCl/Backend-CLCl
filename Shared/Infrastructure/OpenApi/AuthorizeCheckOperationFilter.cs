// Shared/Infrastructure/OpenApi/AuthorizeCheckOperationFilter.cs
using Microsoft.OpenApi.Models;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FacturasIA.Platform.Shared.Infrastructure.OpenApi;

/// <summary>
///     Agrega el requisito de seguridad Bearer a una operación de Swagger a menos que la acción
///     esté decorada con [AllowAnonymous], para que el candado solo aparezca en los endpoints
///     que realmente requieren JWT.
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();
        if (allowAnonymous) return;

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    }
}