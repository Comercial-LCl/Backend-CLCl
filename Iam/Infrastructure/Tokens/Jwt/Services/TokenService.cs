// Iam/Infrastructure/Tokens/Jwt/Services/TokenService.cs
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using FacturasIA.Platform.Iam.Application.Internal.OutboundServices;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;

namespace FacturasIA.Platform.Iam.Infrastructure.Tokens.Jwt.Services;

public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    public string GenerateToken(Usuario usuario)
    {
        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Sid, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre)
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public async Task<Guid?> ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
        try
        {
            var validationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            });

            if (!validationResult.IsValid)
                return null;

            var jwtToken = (JsonWebToken)validationResult.SecurityToken;
            return Guid.Parse(jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
        }
        catch (Exception)
        {
            return null;
        }
    }
}