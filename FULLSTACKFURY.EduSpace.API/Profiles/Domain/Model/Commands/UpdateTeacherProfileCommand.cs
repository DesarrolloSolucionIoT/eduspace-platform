namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;

public record UpdateTeacherProfileCommand(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone);