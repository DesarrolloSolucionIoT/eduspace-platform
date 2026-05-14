namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class ClassroomNotFoundForMeetingException : Exception
{
    public ClassroomNotFoundForMeetingException(int classroomId)
        : base($"Classroom with ID {classroomId} does not exist.") { }
}
