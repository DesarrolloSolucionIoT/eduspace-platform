using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;

public interface IMeetingCommandService
{
    Task<Meeting?> Handle(CreateMeetingCommand command);
    Task Handle(DeleteMeetingCommand command);
    Task<Meeting?> Handle(UpdateMeetingCommand command);
    Task Handle(AddTeacherToMeetingCommand command);
    Task Handle(RemoveTeacherFromMeetingCommand command);
}