using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class RefreshTokenRepository(AppDbContext context)
    : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> FindByHashAsync(string hash)
    {
        return await Context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash);
    }

    public async Task<IEnumerable<RefreshToken>> FindActiveByAccountIdAsync(int accountId)
    {
        return await Context.Set<RefreshToken>()
            .Where(rt => rt.AccountId == accountId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAllForAccountAsync(int accountId)
    {
        var activeTokens = await Context.Set<RefreshToken>()
            .Where(rt => rt.AccountId == accountId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
            token.Revoke();
    }
}
