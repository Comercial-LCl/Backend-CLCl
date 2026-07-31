// Iam/Interfaces/Rest/Resources/SignUpResource.cs
namespace FacturasIA.Platform.Iam.Interfaces.Rest.Resources;

public record SignUpResource(string Nombre, string Email, string Password, string? RucNegocio);