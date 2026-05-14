namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class InvalidResourceDataException : Exception
{
    public InvalidResourceDataException(string message)
        : base(message) { }
}
