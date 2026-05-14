namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class InvalidSharedAreaDataException : Exception
{
    public InvalidSharedAreaDataException(string message)
        : base(message) { }
}
