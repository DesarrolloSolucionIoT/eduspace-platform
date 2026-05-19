using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class ActivationTokenRepository(AppDbContext context)
    : BaseRepository<ActivationToken>(context), IActivationTokenRepository
{
    public async Task<ActivationToken?> FindActiveByHashAsync(string hash)
    {
        return await Context.Set<ActivationToken>()
            .FirstOrDefaultAsync(at => at.TokenHash == hash && at.UsedAt == null && at.ExpiresAt > DateTime.UtcNow);
    }
}
