using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.Services;

public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IOptions<TokenSettings> tokenSettings,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    private readonly TimeSpan _refreshTokenLifetime =
        TimeSpan.FromDays(tokenSettings.Value.RefreshTokenLifetimeDays);

    public async Task<(string rawToken, RefreshToken entity)> CreateForAccountAsync(int accountId)
    {
        var (entity, rawToken) = RefreshToken.CreateNew(accountId, _refreshTokenLifetime);
        await refreshTokenRepository.AddAsync(entity);
        await unitOfWork.CompleteAsync();

        logger.LogDebug("Refresh token created for account {AccountId}", accountId);
        return (rawToken, entity);
    }

    public async Task<(string newRaw, RefreshToken newEntity, RefreshToken oldEntity)> RotateAsync(string rawToken)
    {
        var hash = RefreshToken.ComputeHash(rawToken);
        var oldToken = await refreshTokenRepository.FindByHashAsync(hash);

        if (oldToken is null)
            throw new RefreshTokenNotFoundException();

        if (oldToken.RevokedAt.HasValue)
            throw new RefreshTokenAlreadyUsedException();

        if (oldToken.ExpiresAt <= DateTime.UtcNow)
            throw new RefreshTokenExpiredException();

        var (newEntity, newRaw) = RefreshToken.CreateNew(oldToken.AccountId, _refreshTokenLifetime);
        await refreshTokenRepository.AddAsync(newEntity);
        await unitOfWork.CompleteAsync();

        oldToken.ReplaceWith(newEntity.Id);
        await unitOfWork.CompleteAsync();

        logger.LogDebug("Refresh token rotated for account {AccountId}", oldToken.AccountId);
        return (newRaw, newEntity, oldToken);
    }

    public async Task RevokeAsync(string rawToken)
    {
        var hash = RefreshToken.ComputeHash(rawToken);
        var token = await refreshTokenRepository.FindByHashAsync(hash);

        if (token is null)
            throw new RefreshTokenNotFoundException();

        token.Revoke();
        await unitOfWork.CompleteAsync();

        logger.LogDebug("Refresh token revoked for account {AccountId}", token.AccountId);
    }
}
