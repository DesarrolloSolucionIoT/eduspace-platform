using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class SharedAreaRepository(AppDbContext context) : BaseRepository<SharedArea>(context), ISharedAreaRepository
{
    /// <Inheritdoc />
    public new async Task<SharedArea?> FindByIdAsync(int id)
    {
        return await Context.Set<SharedArea>()
            .FirstOrDefaultAsync(sharedArea => sharedArea.Id == id);
    }

    /// <Inheritdoc />
    public new async Task<IEnumerable<SharedArea>> ListAsync()
    {
        return await Context.Set<SharedArea>()
            .ToListAsync();
    }

    /// <Inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await Context.Set<SharedArea>()
            .AnyAsync(sharedArea => sharedArea.Name == name);
    }
}