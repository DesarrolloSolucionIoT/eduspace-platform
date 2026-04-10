namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

public record CreateTeacherProfileResource(
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone,
    int AdministratorId,
    string Username,
    string Password)
{
}