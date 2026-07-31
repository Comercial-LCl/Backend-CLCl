// Invoicing/Interfaces/Rest/Transform/InvoicingActionResultAssembler.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FacturasIA.Platform.Invoicing.Domain.Model;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Application.Model;
using FacturasIA.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class InvoicingActionResultAssembler
{
    private static int ToStatusCodeFromInvoicingError(InvoicingError error)
    {
        return error switch
        {
            InvoicingError.FacturaNotFound => StatusCodes.Status404NotFound,
            InvoicingError.NoAutorizado => StatusCodes.Status403Forbidden,
            InvoicingError.ProcesamientoIaFallido => StatusCodes.Status502BadGateway,
            InvoicingError.AlmacenamientoFallido => StatusCodes.Status502BadGateway,
            InvoicingError.OperationCancelled => StatusCodes.Status409Conflict,
            InvoicingError.DatabaseError => StatusCodes.Status500InternalServerError,
            InvoicingError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromFacturaResult(
        ControllerBase controller,
        Result<Factura> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Factura, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromInvoicingError((InvoicingError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}