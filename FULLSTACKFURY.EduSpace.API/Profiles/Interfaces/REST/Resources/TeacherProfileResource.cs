namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

public record TeacherProfileResource(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone);