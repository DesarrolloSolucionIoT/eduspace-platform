using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Domain.Services;

public interface IReservationCommandService
{
    Task<Reservation?> Handle(CreateReservationCommand command);
    Task<Reservation?> Handle(UpdateReservationCommand command);
    Task Handle(DeleteReservationCommand command);
}