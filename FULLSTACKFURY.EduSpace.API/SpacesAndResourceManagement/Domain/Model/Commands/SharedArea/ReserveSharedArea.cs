namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

public record ReserveSharedAreaCommand(
    int SharedAreaId,
    int TeacherId,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Reason
);