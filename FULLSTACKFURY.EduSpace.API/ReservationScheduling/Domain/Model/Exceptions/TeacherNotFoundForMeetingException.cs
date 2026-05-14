namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class TeacherNotFoundForMeetingException : Exception
{
    public TeacherNotFoundForMeetingException(int teacherId)
        : base($"Teacher with ID {teacherId} does not exist.") { }
}
