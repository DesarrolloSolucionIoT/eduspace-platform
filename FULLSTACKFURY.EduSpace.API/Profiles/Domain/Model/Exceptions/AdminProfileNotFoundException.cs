namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

public class AdminProfileNotFoundException : ProfileNotFoundException
{
    public AdminProfileNotFoundException(int id)
        : base($"Administrator profile with ID {id} not found.")
    {
    }
}
