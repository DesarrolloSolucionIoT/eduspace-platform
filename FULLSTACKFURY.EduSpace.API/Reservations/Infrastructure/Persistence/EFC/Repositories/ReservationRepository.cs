using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Infrastructure.Persistence.EFC.Repositories;

public class ReservationRepository(AppDbContext context)
    : BaseRepository<Reservation>(context), IReservationRepository

{
    public async Task<IEnumerable<Reservation>> FindAllByAreaIdAsync(int areaId)
    {
        return await Context.Set<Reservation>().Where(f => f.AreaId.Identifier == areaId).ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> FindAllByAreaIdMonthAndDayAsync(int areaId, int month, int day)
    {
        return await Context.Set<Reservation>().Where(f =>
            f.AreaId.Identifier == areaId && f.ReservationDate.Start.Month == month &&
            f.ReservationDate.Start.Day == day).ToListAsync();
    }
}