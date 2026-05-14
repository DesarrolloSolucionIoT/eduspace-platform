namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class TeacherAlreadyInMeetingException : Exception
{
    public TeacherAlreadyInMeetingException(int teacherId, int meetingId)
        : base($"Teacher with ID {teacherId} is already a participant of meeting {meetingId}.") { }
}
