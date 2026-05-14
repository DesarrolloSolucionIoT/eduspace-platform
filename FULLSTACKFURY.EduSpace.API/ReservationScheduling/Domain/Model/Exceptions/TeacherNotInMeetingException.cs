namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class TeacherNotInMeetingException : Exception
{
    public TeacherNotInMeetingException(int teacherId, int meetingId)
        : base($"Teacher with ID {teacherId} is not a participant of meeting {meetingId}.") { }
}
