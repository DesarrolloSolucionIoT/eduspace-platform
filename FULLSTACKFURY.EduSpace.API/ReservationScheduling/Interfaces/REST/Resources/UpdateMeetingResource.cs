namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

public record UpdateMeetingResource(
    int MeetingId,
    string Title,
    string Description,
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End,
    int AdministratorId,
    int ClassroomId
);