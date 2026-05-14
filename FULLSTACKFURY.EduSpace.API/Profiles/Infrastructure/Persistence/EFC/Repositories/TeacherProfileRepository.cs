using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class TeacherProfileRepository(AppDbContext context)
    : BaseRepository<TeacherProfile>(context), ITeacherProfileRepository
{
    public bool ExistsByTeacherProfileId(int teacherProfileId)
    {
        return Context.Set<TeacherProfile>().Any(tp => tp.Id == teacherProfileId);
    }

    public async Task<TeacherProfile?> FindByAccountIdAsync(int accountId)
    {
        return await Context.Set<TeacherProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(tp => tp.AccountId.Id == accountId);
    }

    public override async Task<IEnumerable<TeacherProfile>> ListAsync()
    {
        return await Context.Set<TeacherProfile>()
            .AsNoTracking()
            .ToListAsync();
    }
}
