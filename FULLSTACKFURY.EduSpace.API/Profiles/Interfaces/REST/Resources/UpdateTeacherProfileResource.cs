namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

public record UpdateTeacherProfileResource(
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone);