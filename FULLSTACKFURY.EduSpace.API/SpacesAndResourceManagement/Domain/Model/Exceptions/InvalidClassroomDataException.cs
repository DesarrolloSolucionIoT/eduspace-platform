namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class InvalidClassroomDataException : Exception
{
    public InvalidClassroomDataException(string message)
        : base(message) { }
}
