namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

//:TODO: It should return a token and the username
public record AdminProfileResource(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string Address,
    string Phone);