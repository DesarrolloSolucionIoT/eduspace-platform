using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class ClassroomRepository(AppDbContext context) : BaseRepository<Classroom>(context), IClassroomRepository
{
    // InheritedDoc
    public async Task<IEnumerable<Classroom>> FindByTeacherIdAsync(int teacherId)
    {
        return await Context.Set<Classroom>()
            .Where(classroom => classroom.TeacherId.TeacherIdentifier == teacherId)
            .ToListAsync();
    }

    // InheritedDoc
    public new async Task<Classroom?> FindByIdAsync(int id)
    {
        return await Context.Set<Classroom>()
            .FirstOrDefaultAsync(classroom => classroom.Id == id);
    }

    // InheritedDoc
    public new async Task<IEnumerable<Classroom>> ListAsync()
    {
        return await Context.Set<Classroom>()
            .ToListAsync();
    }

    // InheritedDoc
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await Context.Set<Classroom>()
            .AnyAsync(classroom => classroom.Name == name);
    }

    public bool ExistsByClassroomId(int id)
    {
        return Context.Set<Classroom>().Any(classroom => classroom.Id == id);
    }

    public bool ExistsByClassroomName(string name)
    {
        return Context.Set<Classroom>().Any(classroom => classroom.Name == name);
    }
}