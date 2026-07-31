// Iam/Interfaces/Rest/Transform/SignUpCommandFromResourceAssembler.cs
using FacturasIA.Platform.Iam.Domain.Model.Commands;
using FacturasIA.Platform.Iam.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Iam.Interfaces.Rest.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        return new SignUpCommand(
            resource.Nombre.Trim(),
            resource.Email.Trim(),
            resource.Password.Trim(),
            resource.RucNegocio?.Trim());
    }
}