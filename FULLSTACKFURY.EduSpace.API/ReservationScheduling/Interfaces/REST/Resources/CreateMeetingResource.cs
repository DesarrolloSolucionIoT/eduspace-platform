namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

public record CreateMeetingResource(
    string Title,
    string Description,
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End
);