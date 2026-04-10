using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class TeacherProfileRepository(AppDbContext context)
    : BaseRepository<TeacherProfile>(context), ITeacherProfileRepository
{
    public async Task<IEnumerable<TeacherProfile>> FindAllTeachersByAdministratorIdAsync(int id)
    {
        return await Context.Set<TeacherProfile>()
            .Where(t => t.AdministratorId == id)
            .ToListAsync();
    }

    public bool ExistsByTeacherProfileId(int teacherProfileId)
    {
        return Context.Set<TeacherProfile>().Any(teacherProfile => teacherProfile.Id == teacherProfileId);
    }

    public override async Task<IEnumerable<TeacherProfile>> ListAsync()
    {
        return await Context.Set<TeacherProfile>()
            .Include(p => p.AccountId)
            .ToListAsync();
    }
}