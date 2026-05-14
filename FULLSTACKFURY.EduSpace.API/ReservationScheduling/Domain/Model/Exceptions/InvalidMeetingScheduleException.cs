namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class InvalidMeetingScheduleException : Exception
{
    public InvalidMeetingScheduleException(string message) : base(message) { }
}
