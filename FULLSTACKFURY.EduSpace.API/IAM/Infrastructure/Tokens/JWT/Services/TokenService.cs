using System.Security.Claims;
using System.Text;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Services;

public class TokenService(
    IOptions<TokenSettings> tokenSettings,
    ILogger<TokenService> logger) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    public string GenerateToken(Account account)
    {
        var secret = _tokenSettings.Secret;
        var key = Encoding.UTF8.GetBytes(secret);
        var now = DateTime.UtcNow;
        var lifetime = TimeSpan.FromMinutes(_tokenSettings.AccessTokenLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, account.Username),
            new(ClaimTypes.Role, account.GetRole())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _tokenSettings.Issuer,
            Audience = _tokenSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(lifetime),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        logger.LogDebug("Access token generated for account {AccountId}", account.Id);
        return token;
    }
}
