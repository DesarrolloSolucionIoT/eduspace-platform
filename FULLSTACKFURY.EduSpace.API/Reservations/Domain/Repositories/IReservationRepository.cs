using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Repositories;

public interface IReservationRepository : IBaseRepository<Reservation>
{
    Task<IEnumerable<Reservation>> FindAllByAreaIdAsync(int areaId);

    Task<IEnumerable<Reservation>> FindAllByAreaIdMonthAndDayAsync(int areaId, int month, int day);
}