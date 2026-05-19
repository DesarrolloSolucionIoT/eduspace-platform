using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class AdminProfileRepository(AppDbContext context)
    : BaseRepository<AdminProfile>(context), IAdminProfileRepository
{
    public bool ExistsByAdminProfileId(int adminProfileId)
    {
        return Context.Set<AdminProfile>().Any(ap => ap.Id == adminProfileId);
    }

    public async Task<AdminProfile?> FindByAccountIdAsync(int accountId)
    {
        return await Context.Set<AdminProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(ap => ap.AccountId.Id == accountId);
    }

    public async Task<int?> FindAccountIdByEmailAsync(string email)
    {
        return await Context.Set<AdminProfile>()
            .AsNoTracking()
            .Where(ap => ap.ProfilePrivateInformation.Email == email)
            .Select(ap => (int?)ap.AccountId.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> FindLinkedAccountIdAsync(int profileId)
    {
        return await Context.Set<AdminProfile>()
            .AsNoTracking()
            .Where(ap => ap.Id == profileId)
            .Select(ap => (int?)ap.AccountId.Id)
            .FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<AdminProfile>> ListAsync()
    {
        return await Context.Set<AdminProfile>()
            .AsNoTracking()
            .ToListAsync();
    }
}
