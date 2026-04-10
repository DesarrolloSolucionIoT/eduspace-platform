using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Application.Internal.QueryServices;

public class ReservationQueryService(IReservationRepository reservationRepository) : IReservationQueryService
{
    public Task<IEnumerable<Reservation>> Handle(GetAllReservationsQuery query)
    {
        return reservationRepository.ListAsync();
    }

    public Task<IEnumerable<Reservation>> Handle(GetAllReservationsByAreaIdQuery query)
    {
        return reservationRepository.FindAllByAreaIdAsync(query.AreaId);
    }

    public Task<IEnumerable<Reservation>> Handle(GetReservationByAreaIdMonthAndDay query)
    {
        return reservationRepository.FindAllByAreaIdMonthAndDayAsync(query.AreaId, query.Month, query.Day);
    }

    public async Task<Reservation?> Handle(GetReservationByIdQuery query)
    {
        return await reservationRepository.FindByIdAsync(query.Id);
    }
}