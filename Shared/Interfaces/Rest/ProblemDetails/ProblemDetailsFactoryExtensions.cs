// Shared/Interfaces/Rest/ProblemDetails/ProblemDetailsFactoryExtensions.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FacturasIA.Platform.Shared.Interfaces.Rest.ProblemDetails;

/// <summary>
///     Extension methods sobre el ProblemDetailsFactory de ASP.NET Core para construir una
///     respuesta de error consistente a partir de un enum de dominio y un mensaje localizado.
/// </summary>
public static class ProblemDetailsFactoryExtensions
{
    public static IActionResult CreateProblemDetails(
        this ProblemDetailsFactory problemDetailsFactory,
        ControllerBase controller,
        int statusCode,
        Enum? error,
        string message)
    {
        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            controller.HttpContext,
            statusCode,
            title: error?.ToString() ?? "Error",
            detail: message
        );

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}