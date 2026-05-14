namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

public class ProfileNotFoundException : Exception
{
    public ProfileNotFoundException(string message) : base(message)
    {
    }
}
