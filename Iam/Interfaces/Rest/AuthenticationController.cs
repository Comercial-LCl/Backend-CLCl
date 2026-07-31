// Iam/Interfaces/Rest/AuthenticationController.cs
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FacturasIA.Platform.Iam.Application.CommandServices;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using FacturasIA.Platform.Iam.Interfaces.Rest.Resources;
using FacturasIA.Platform.Iam.Interfaces.Rest.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace FacturasIA.Platform.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Endpoints de autenticación.")]
public class AuthenticationController(
    IUsuarioCommandService usuarioCommandService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("Iniciar sesión", "Autentica a un usuario con email y contraseña.", OperationId = "SignIn")]
    [SwaggerResponse(200, "El usuario fue autenticado.", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(401, "Email o contraseña incorrectos.")]
    public async Task<IActionResult> SignIn(SignInResource resource, CancellationToken cancellationToken)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await usuarioCommandService.Handle(signInCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromSignInResult(
            this,
            result,
            problemDetailsFactory,
            authenticated => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                authenticated.Usuario, authenticated.Token))
        );
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation("Registrarse", "Registra una nueva cuenta de dueño de negocio.", OperationId = "SignUp")]
    [SwaggerResponse(200, "El usuario fue creado correctamente.")]
    [SwaggerResponse(409, "El email ya está registrado.")]
    public async Task<IActionResult> SignUp(SignUpResource resource, CancellationToken cancellationToken)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await usuarioCommandService.Handle(signUpCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromSignUpResult(
            this,
            result,
            problemDetailsFactory,
            () => Ok(new { message = "Usuario creado correctamente." })
        );
    }
}