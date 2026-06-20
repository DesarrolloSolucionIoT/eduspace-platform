namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

public record SharedAreaReservationResource(
    int Id,
    int SharedAreaId,
    string SharedAreaName,
    int TeacherId,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Reason,
    DateTime CreatedAt
); 