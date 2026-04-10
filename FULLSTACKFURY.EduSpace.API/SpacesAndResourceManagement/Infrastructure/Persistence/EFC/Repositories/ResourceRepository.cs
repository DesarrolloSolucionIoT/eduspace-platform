using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class ResourceRepository(AppDbContext context) : BaseRepository<Resource>(context), IResourceRepository
{
    // inheritedDoc
    public async Task<IEnumerable<Resource>> FindByClassroomIdAsync(int classroomId)
    {
        return await Context.Set<Resource>()
            .Include(resource => resource.Classroom)
            .Where(resource => resource.ClassroomId == classroomId)
            .ToListAsync();
    }

    // inheritedDoc
    public override async Task<Resource?> FindByIdAsync(int id)
    {
        return await Context.Set<Resource>()
            .Include(resource => resource.Classroom)
            .FirstOrDefaultAsync(resource => resource.Id == id);
    }

    // inheritedDoc
    public override async Task<IEnumerable<Resource>> ListAsync()
    {
        return await Context.Set<Resource>()
            .Include(resource => resource.Classroom)
            .ToListAsync();
    }

    // inheritedDoc
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await Context.Set<Resource>()
            .AnyAsync(resource => resource.Name == name);
    }
}