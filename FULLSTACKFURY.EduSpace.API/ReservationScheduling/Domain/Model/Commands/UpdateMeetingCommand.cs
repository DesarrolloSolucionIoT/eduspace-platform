namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Commands;

public record UpdateMeetingCommand(
    int MeetingId,
    string Title,
    string Description,
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End,
    int AdministratorId,
    int ClassroomId);