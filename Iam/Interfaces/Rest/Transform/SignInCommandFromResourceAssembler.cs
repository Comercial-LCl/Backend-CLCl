// Iam/Interfaces/Rest/Transform/SignInCommandFromResourceAssembler.cs
using FacturasIA.Platform.Iam.Domain.Model.Commands;
using FacturasIA.Platform.Iam.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Iam.Interfaces.Rest.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email.Trim(), resource.Password.Trim());
    }
}