// Iam/Interfaces/Rest/Transform/IamActionResultAssembler.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FacturasIA.Platform.Iam.Domain.Model;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Shared.Application.Model;
using FacturasIA.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace FacturasIA.Platform.Iam.Interfaces.Rest.Transform;

public static class IamActionResultAssembler
{
    private static int ToStatusCodeFromIamError(IamError error)
    {
        return error switch
        {
            IamError.InvalidCredentials => StatusCodes.Status401Unauthorized,
            IamError.EmailAlreadyTaken => StatusCodes.Status409Conflict,
            IamError.UsuarioNotFound => StatusCodes.Status404NotFound,
            IamError.OperationCancelled => StatusCodes.Status409Conflict,
            IamError.DatabaseError => StatusCodes.Status500InternalServerError,
            IamError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromSignUpResult(
        ControllerBase controller,
        Result result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction();

        var statusCode = ToStatusCodeFromIamError((IamError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromSignInResult(
        ControllerBase controller,
        Result<(Usuario Usuario, string Token)> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<(Usuario Usuario, string Token), IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value);

        var statusCode = ToStatusCodeFromIamError((IamError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}