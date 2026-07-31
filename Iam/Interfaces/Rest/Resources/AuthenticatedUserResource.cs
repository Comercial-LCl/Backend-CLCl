// Iam/Interfaces/Rest/Resources/AuthenticatedUserResource.cs
namespace FacturasIA.Platform.Iam.Interfaces.Rest.Resources;

public record AuthenticatedUserResource(
    Guid Id,
    string Nombre,
    string Email,
    string? RucNegocio,
    string Token);