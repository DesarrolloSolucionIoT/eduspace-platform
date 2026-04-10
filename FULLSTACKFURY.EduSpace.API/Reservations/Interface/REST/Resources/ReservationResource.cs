namespace FULLSTACKFURY.EduSpace.API.Reservations.Interface.REST.Resources;

public record ReservationResource(int Id, DateTime Start, DateTime End, string Title, int AreaId, int TeacherId);