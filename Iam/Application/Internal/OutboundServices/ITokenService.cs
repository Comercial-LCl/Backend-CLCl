// Iam/Application/Internal/OutboundServices/ITokenService.cs
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;

namespace FacturasIA.Platform.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service usado para generar y validar JWTs
/// </summary>
public interface ITokenService
{
    string GenerateToken(Usuario usuario);
    Task<Guid?> ValidateToken(string token);
}