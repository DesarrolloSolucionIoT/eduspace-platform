namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

public class SharedAreaReservationConflictException : Exception
{
    public SharedAreaReservationConflictException(int sharedAreaId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        : base($"Shared area with ID {sharedAreaId} is already reserved on {date} from {startTime} to {endTime}.") { }

    public SharedAreaReservationConflictException(string message)
        : base(message) { }
}