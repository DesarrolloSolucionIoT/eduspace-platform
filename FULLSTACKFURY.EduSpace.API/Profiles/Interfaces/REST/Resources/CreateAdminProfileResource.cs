namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

public record CreateAdminProfileResource(
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone,
    string Username,
    string Password);