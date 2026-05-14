using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class ClassroomRepository(AppDbContext context) : BaseRepository<Classroom>(context), IClassroomRepository
{
    public async Task<IEnumerable<Classroom>> FindByTeacherIdAsync(int teacherId)
    {
        return await Context.Set<Classroom>()
            .AsNoTracking()
            .Where(c => c.TeacherId.TeacherIdentifier == teacherId)
            .ToListAsync();
    }

    public new async Task<Classroom?> FindByIdAsync(int id)
    {
        return await Context.Set<Classroom>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public new async Task<IEnumerable<Classroom>> ListAsync()
    {
        return await Context.Set<Classroom>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await Context.Set<Classroom>()
            .AnyAsync(c => c.Name == name);
    }

    public async Task<bool> ExistsByClassroomIdAsync(int id)
    {
        return await Context.Set<Classroom>()
            .AnyAsync(c => c.Id == id);
    }
}
