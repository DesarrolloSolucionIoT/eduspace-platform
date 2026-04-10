namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

public record AuthenticatedAccountResource(
    int Id,
    int? ProfileId,
    string Username,
    string Role,
    string Token,
    ProfileData? Profile,
    IEnumerable<ClassroomData>? Classrooms,
    IEnumerable<MeetingData>? Meetings
);

public record ProfileData(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone,
    int? AdministratorId
);

public record ClassroomData(
    int Id,
    string Name,
    string Description
);

public record MeetingData(
    int MeetingId,
    string Title,
    string Description,
    DateOnly Date,
    TimeOnly Start,
    TimeOnly End
);