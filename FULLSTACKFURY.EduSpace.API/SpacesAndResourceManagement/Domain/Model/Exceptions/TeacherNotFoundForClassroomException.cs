namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

/// <summary>
///     Thrown when an external profile verification fails for a teacher being assigned to a classroom.
///     Maps to HTTP 400 because the caller supplied an ID that does not correspond to a valid teacher profile —
///     this is a client data error, not a missing resource the server owns.
/// </summary>
public class TeacherNotFoundForClassroomException : Exception
{
    public TeacherNotFoundForClassroomException(int teacherId)
        : base($"Teacher profile with ID {teacherId} was not found or is not a valid teacher.") { }

    public TeacherNotFoundForClassroomException(string message)
        : base(message) { }
}
