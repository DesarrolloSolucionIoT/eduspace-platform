namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class MeetingNotFoundException : Exception
{
    public MeetingNotFoundException(int meetingId)
        : base($"Meeting with ID {meetingId} was not found.") { }
}
