// Iam/Interfaces/Rest/Transform/AuthenticatedUserResourceFromEntityAssembler.cs
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Iam.Interfaces.Rest.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(Usuario entity, string token)
    {
        return new AuthenticatedUserResource(
            entity.Id,
            entity.Nombre,
            entity.Email,
            entity.RucNegocio,
            token);
    }
}