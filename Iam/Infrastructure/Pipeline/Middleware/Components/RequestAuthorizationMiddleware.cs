// Iam/Infrastructure/Pipeline/Middleware/Components/RequestAuthorizationMiddleware.cs
using FacturasIA.Platform.Iam.Application.QueryServices;
using FacturasIA.Platform.Iam.Application.Internal.OutboundServices;
using FacturasIA.Platform.Iam.Domain.Model.Queries;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUsuarioQueryService usuarioQueryService,
        ITokenService tokenService)
    {
        var cancellationToken = context.RequestAborted;

        var allowAnonymous = context.GetEndpoint()?.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;
        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').Last();
        var usuarioId = token is null ? null : await tokenService.ValidateToken(token);

        if (usuarioId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var usuario = await usuarioQueryService.Handle(new GetUsuarioByIdQuery(usuarioId.Value), cancellationToken);
        context.Items["User"] = usuario;

        await next(context);
    }
}