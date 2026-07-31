// Iam/Application/Internal/OutboundServices/IHashingService.cs
namespace FacturasIA.Platform.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service usado para hashear y verificar contraseñas
/// </summary>
public interface IHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}