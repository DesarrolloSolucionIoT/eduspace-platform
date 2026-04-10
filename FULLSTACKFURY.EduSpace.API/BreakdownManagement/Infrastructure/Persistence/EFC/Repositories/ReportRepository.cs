using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Infrastructure.Persistence.EFC.Repositories;

public class ReportRepository(AppDbContext context) : BaseRepository<Report>(context), IReportRepository
{
    public override async Task<Report?> FindByIdAsync(int id)
    {
        return await Context.Set<Report>().FindAsync(id);
    }

    public async Task<IEnumerable<Report>> FindAllAsync()
    {
        return await Context.Set<Report>().ToListAsync();
    }

    public async Task<IEnumerable<Report>> FindAllByResourceIdAsync(int resourceId)
    {
        return await Context.Set<Report>()
            .Where(r => r.ResourceId == new ResourceId(resourceId))
            .ToListAsync();
    }
}