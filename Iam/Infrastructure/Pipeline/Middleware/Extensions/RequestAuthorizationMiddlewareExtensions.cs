// Iam/Infrastructure/Pipeline/Middleware/Extensions/RequestAuthorizationMiddlewareExtensions.cs
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}