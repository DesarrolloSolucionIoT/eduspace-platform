namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

/// <summary>

/// Represents the command to reserve a shared area.

/// </summary>

/// <param name="SharedAreaId">The ID of the shared area to reserve.</param>

/// <param name="TeacherId">The ID of the teacher reserving the shared area.</param>

/// <param name="ReservationDate">The date for the reservation.</param>

/// <param name="StartTime">The start time of the reservation.</param>

/// <param name="EndTime">The end time of the reservation.</param>
/// 
/// <param name="Reason">The reason for the reservation.</param>


public record ReserveSharedAreaCommand(
    int SharedAreaId,
    int TeacherId,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Reason
);