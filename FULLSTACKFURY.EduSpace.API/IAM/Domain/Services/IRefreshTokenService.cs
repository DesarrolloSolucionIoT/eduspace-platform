using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;

public interface IRefreshTokenService
{
    /// <summary>Creates a new refresh token for the given account. Returns the raw token and the persisted entity.</summary>
    Task<(string rawToken, RefreshToken entity)> CreateForAccountAsync(int accountId);

    /// <summary>
    /// Rotates a refresh token: revokes the old one, issues a new one.
    /// Throws <see cref="RefreshTokenNotFoundException"/> if not found.
    /// Throws <see cref="RefreshTokenAlreadyUsedException"/> if already revoked.
    /// Throws <see cref="RefreshTokenExpiredException"/> if expired.
    /// </summary>
    Task<(string newRaw, RefreshToken newEntity, RefreshToken oldEntity)> RotateAsync(string rawToken);

    /// <summary>
    /// Revokes a refresh token.
    /// Throws <see cref="RefreshTokenNotFoundException"/> if not found.
    /// </summary>
    Task RevokeAsync(string rawToken);
}
