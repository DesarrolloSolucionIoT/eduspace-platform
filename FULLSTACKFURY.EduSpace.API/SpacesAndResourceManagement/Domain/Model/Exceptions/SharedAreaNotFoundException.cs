namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class SharedAreaNotFoundException : Exception
{
    public SharedAreaNotFoundException(int id)
        : base($"Shared area with ID {id} was not found.") { }

    public SharedAreaNotFoundException(string message)
        : base(message) { }
}
