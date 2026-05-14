namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class ClassroomNotFoundException : Exception
{
    public ClassroomNotFoundException(int id)
        : base($"Classroom with ID {id} was not found.") { }

    public ClassroomNotFoundException(string message)
        : base(message) { }
}
