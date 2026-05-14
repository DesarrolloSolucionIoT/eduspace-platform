namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(int id)
        : base($"Resource with ID {id} was not found.") { }

    public ResourceNotFoundException(string message)
        : base(message) { }
}
