using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

public record MeetingResource(
    int MeetingId,
    string Title,
    string Description,
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End,
    AdministratorId AdministratorId,
    ClassroomId ClassroomId,
    IEnumerable<TeacherResource> Teachers
);

public record TeacherResource(
    int Id,
    string FirstName,
    string LastName
);