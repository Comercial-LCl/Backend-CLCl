// Shared/Interfaces/Rest/ControllerBaseExtensions.cs
using Microsoft.AspNetCore.Mvc;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;

namespace FacturasIA.Platform.Shared.Interfaces.Rest;

public static class ControllerBaseExtensions
{
    public static Guid CurrentUserId(this ControllerBase controller)
    {
        var user = (Usuario?)controller.HttpContext.Items["User"];
        return user?.Id ?? throw new InvalidOperationException("No authenticated user in context.");
    }
}