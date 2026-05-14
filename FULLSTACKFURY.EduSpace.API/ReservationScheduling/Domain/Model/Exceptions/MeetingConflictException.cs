namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Exceptions;

public class MeetingConflictException : Exception
{
    public MeetingConflictException(int teacherId, DateOnly date)
        : base($"Teacher with ID {teacherId} already has a conflicting meeting on {date}.") { }

    public MeetingConflictException(string message) : base(message) { }
}
