using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Infrastructure.Persistence.EFC.Repositories;

public class SharedAreaReservationRepository(AppDbContext context)
    : BaseRepository<SharedAreaReservation>(context), ISharedAreaReservationRepository
{
    public async Task<bool> ExistsBySharedAreaAndDateTimeRangeAsync(
        int sharedAreaId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return await Context.Set<SharedAreaReservation>()
            .AnyAsync(r =>
                r.SharedAreaId == sharedAreaId &&
                r.ReservationDate == date &&
                r.StartTime < endTime &&
                r.EndTime > startTime);
    }

    public async Task<IEnumerable<SharedAreaReservation>> FindBySharedAreaIdAndDateAsync(int sharedAreaId, DateOnly date)
    {
        return await Context.Set<SharedAreaReservation>()
            .AsNoTracking()
            .Where(r => r.SharedAreaId == sharedAreaId && r.ReservationDate == date)
            .ToListAsync();
    }

    public async Task<IEnumerable<SharedAreaReservation>> FindByTeacherIdAsync(int teacherId)
    {
        return await Context.Set<SharedAreaReservation>()
            .AsNoTracking()
            .Include(r => r.SharedArea)
            .Where(r => r.TeacherId == teacherId)
            .ToListAsync();
    }
}
