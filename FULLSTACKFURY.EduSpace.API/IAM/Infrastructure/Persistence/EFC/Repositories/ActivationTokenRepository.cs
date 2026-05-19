using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class ActivationTokenRepository(AppDbContext context)
    : BaseRepository<ActivationToken>(context), IActivationTokenRepository
{
    public async Task<ActivationToken?> FindByHashAsync(string hash)
    {
        return await Context.Set<ActivationToken>()
            .FirstOrDefaultAsync(at => at.TokenHash == hash);
    }

    public async Task DeleteAllForAccountAsync(int accountId)
    {
        var tokens = await Context.Set<ActivationToken>()
            .Where(at => at.AccountId == accountId)
            .ToListAsync();

        Context.Set<ActivationToken>().RemoveRange(tokens);
    }
}
