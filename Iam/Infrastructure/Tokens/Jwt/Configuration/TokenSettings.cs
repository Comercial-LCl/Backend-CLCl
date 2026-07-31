// Iam/Infrastructure/Tokens/Jwt/Configuration/TokenSettings.cs
namespace FacturasIA.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     Configuración JWT, se enlaza desde la sección "TokenSettings" de appsettings.json
/// </summary>
public class TokenSettings
{
    public required string Secret { get; set; }
}