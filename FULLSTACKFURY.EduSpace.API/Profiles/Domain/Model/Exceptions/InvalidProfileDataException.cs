namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;

public class InvalidProfileDataException : Exception
{
    public InvalidProfileDataException(string message) : base(message)
    {
    }
}
