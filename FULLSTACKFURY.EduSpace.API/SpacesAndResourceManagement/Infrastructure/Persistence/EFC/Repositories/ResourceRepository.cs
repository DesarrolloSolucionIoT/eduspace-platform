using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class ResourceRepository(AppDbContext context) : BaseRepository<Resource>(context), IResourceRepository
{
    public async Task<IEnumerable<Resource>> FindByClassroomIdAsync(int classroomId)
    {
        return await Context.Set<Resource>()
            .AsNoTracking()
            .Include(r => r.Classroom)
            .Where(r => r.ClassroomId == classroomId)
            .ToListAsync();
    }

    public override async Task<Resource?> FindByIdAsync(int id)
    {
        return await Context.Set<Resource>()
            .AsNoTracking()
            .Include(r => r.Classroom)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<IEnumerable<Resource>> ListAsync()
    {
        return await Context.Set<Resource>()
            .AsNoTracking()
            .Include(r => r.Classroom)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAndClassroomIdAsync(string name, int classroomId)
    {
        return await Context.Set<Resource>()
            .AnyAsync(r => r.Name == name && r.ClassroomId == classroomId);
    }

    public async Task<bool> ExistsByIdAsync(int resourceId)
    {
        return await Context.Set<Resource>()
            .AnyAsync(r => r.Id == resourceId);
    }
}
