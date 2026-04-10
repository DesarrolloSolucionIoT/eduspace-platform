namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;

public record UpdateAdminProfileCommand(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone);